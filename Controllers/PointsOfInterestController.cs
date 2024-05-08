using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers;

[Route("api/cities/{cityId:int}/[controller]")]
[ApiController]
public class PointsOfInterestController : ControllerBase
{
    private readonly ILogger<PointsOfInterestController> _logger;
    private readonly IMailService _mailService;

    public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
    {
        // throw new Exception("An exception has occured");
        try
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

            if (city != null) return Ok(city.PointsOfInterest);

            _logger.LogWarning($@"City with id {cityId} was not found when accessing points of interest");
            return NotFound();
        }
        catch (Exception e)
        {
            _logger.LogCritical($"Exception when getting points of interest for city with id {cityId}", e);
            return StatusCode(500, "A problem occured when handling your request");
        }
    }

    [HttpGet("{pointOfInterestId:int}", Name = "GetPointOfInterest")]
    public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(point => point.Id == pointOfInterestId);

        if (pointOfInterest == null)
        {
            return NotFound();
        }

        return Ok(pointOfInterest);
    }

    [HttpPost]
    public ActionResult<PointOfInterestDto> CreatePointOfInterest(
        int cityId,
        [FromBody] PointOfInterestForCreationDto pointOfInterest
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }
        
        // demo purposes
        var maxPointOfInterest = CitiesDataStore.Current.Cities.SelectMany(cityDto => cityDto.PointsOfInterest)
            .Max(point => point.Id);

        var finalPointOfInterest = new PointOfInterestDto()
        {
            Id = ++maxPointOfInterest,
            Name = pointOfInterest.Name,
            Description = pointOfInterest.Description
        };
        
        city.PointsOfInterest.Add(finalPointOfInterest);
        
        return CreatedAtRoute("GetPointOfInterest", 
            new
            {
                cityId,
                pointOfInterestId = finalPointOfInterest.Id,
            }, finalPointOfInterest);
    }

    [HttpPut("{pointOfInterestId:int}")]
    public ActionResult<PointOfInterestDto> UpdatePointOfInterest(
        int cityId, 
        int pointOfInterestId,
        [FromBody] PointOfInterestForUpdateDto pointOfInterest)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(point => point.Id == pointOfInterestId);

        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        pointOfInterestFromStore.Name = pointOfInterest.Name;
        pointOfInterestFromStore.Description = pointOfInterest.Description;

        return NoContent();
    }

    [HttpPatch("{pointOfInterestId:int}")]
    public ActionResult<PointOfInterestDto> PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
        JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(point => point.Id == pointOfInterestId);

        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        {
            Name = pointOfInterestFromStore.Name,
            Description = pointOfInterestFromStore.Description
        };
        
        patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(pointOfInterestToPatch))
        {
            return BadRequest(ModelState);
        }

        pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;
        
        return NoContent();
    }

    [HttpDelete("{pointOfInterestId:int}")]
    public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
    {
        var city = CitiesDataStore.Current.Cities.FirstOrDefault(city => city.Id == cityId);

        if (city == null)
        {
            return NotFound();
        }

        var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(point => point.Id == pointOfInterestId);

        if (pointOfInterestFromStore == null)
        {
            return NotFound();
        }

        city.PointsOfInterest.Remove(pointOfInterestFromStore);
        
        _mailService.Send("Point of interest deleted", $"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} has been deleted");

        return NoContent();
    }
}