using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId:int}/[controller]")]
[ApiController]
public class PointsOfInterestController(
    ILogger<PointsOfInterestController> logger,
    IMailService mailService,
    ICityInfoRepository cityInfoRepository,
    IMapper mapper)
    : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMailService _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    private readonly ICityInfoRepository _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
    {
        // throw new Exception("An exception has occured");
        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
        if (!cityExists)
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }
        var pointOfInterestsForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

        return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfInterestsForCity));
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public async Task<ActionResult<PointOfInterestDto>> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
        if (!cityExists)
        {
            _logger.LogInformation($"City with id {cityId} was not found when accessing point of interest with id {pointOfInterestId}");
            return NotFound();
        }

        var pointOfInterest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        
        if (pointOfInterest == null) return NotFound();
        
        return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
    }

    [HttpPost]
    public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest
    )
    {
        if (!ModelState.IsValid) return BadRequest();

        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);

        if (!cityExists) return NotFound();

        var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);

        await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);

        await _cityInfoRepository.SaveChangesAsync();

        var createdPointOfInterest = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);
        
        return CreatedAtRoute("GetPointOfInterest", 
            new
            {
                cityId,
                pointOfInterestId = createdPointOfInterest.Id,
            }, createdPointOfInterest);
    }
    
    
    [HttpPut("{pointOfInterestId:int}")]
    public async Task<ActionResult> UpdatePointOfInterest(
        int cityId, 
        int pointOfInterestId,
        [FromBody] PointOfInterestForUpdateDto pointOfInterest)
    {
        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);

        if (!cityExists) return NotFound();


        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
        
        if (pointOfInterestEntity == null) return NotFound();
        
        _mapper.Map(pointOfInterest, pointOfInterestEntity);

        await _cityInfoRepository.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPatch("{pointOfInterestId:int}")]
    public async Task <ActionResult> PartiallyUpdatePointOfInterest(
        int cityId,
        int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
    
        if (!cityExists) return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

        if (pointOfInterestEntity == null) return NotFound();

        var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
    
        if (!(ModelState.IsValid && TryValidateModel(pointOfInterestToPatch))) return BadRequest(ModelState);

        _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{pointOfInterestId:int}")]
    public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var cityExists = await _cityInfoRepository.CityExistsAsync(cityId);
    
        if (!cityExists) return NotFound();

        var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
    
        if (pointOfInterestEntity == null) return NotFound();
        
        _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
        await _cityInfoRepository.SaveChangesAsync();
        
        _mailService.Send("Point of interest deleted", $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} has been deleted");
    
        return NoContent();
    }
}