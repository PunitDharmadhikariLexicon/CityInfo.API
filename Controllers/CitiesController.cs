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

            var results = cityEntities.Select(cityEntity => new CityWithoutPointsOfInterestDto()
            {
                Id = cityEntity.Id,
                Description = cityEntity.Description,
                Name = cityEntity.Name 
                
            }).ToList();

            return Ok(results);
        }

        [HttpGet("{id:int}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            // var city = _citiesDataStore.Cities.FirstOrDefault(city => city.Id == id);
            //
            // if (city != null) return Ok(city);
            // return NotFound();
            return Ok();
        }
    }
}
