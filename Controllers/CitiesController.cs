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
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();

            // var results = cityEntities.Select(cityEntity => new CityWithoutPointsOfInterestDto()
            // {
            //     Id = cityEntity.Id,
            //     Description = cityEntity.Description,
            //     Name = cityEntity.Name 
            //     
            // }).ToList();

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            
            if (city == null)
            {
                return NotFound();
            }

            return includePointsOfInterest ? Ok(_mapper.Map<CityDto>(city)) : Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
