using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper) : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private const int MaxCitiesPageSize = 20;
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities(
            [FromQuery(Name = "name")] string? name,
            [FromQuery(Name = "searchQuery")] string? searchQuery,
            [FromQuery(Name = "pageNumber")] int pageNumber = 1,
            [FromQuery(Name = "pageSize")] int pageSize = 10
        )
        {
            if (pageSize > MaxCitiesPageSize) pageSize = MaxCitiesPageSize;
            
            var (cityEntities, paginationMetadata) = await _cityInfoRepository.GetCitiesAsync(
                name, 
                searchQuery, 
                pageNumber, 
                pageSize
            );
            
            Response.Headers.Append("X-Pagination-TotalItemCount", paginationMetadata.TotalItemCount.ToString());
            Response.Headers.Append("X-Pagination-TotalPageCount", paginationMetadata.TotalPageCount.ToString());
            Response.Headers.Append("X-Pagination-PageSize", paginationMetadata.PageSize.ToString());
            Response.Headers.Append("X-Pagination-CurrentPage", paginationMetadata.CurrentPage.ToString());

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            
            if (city == null) return NotFound();

            return includePointsOfInterest ? Ok(_mapper.Map<CityDto>(city)) : Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
