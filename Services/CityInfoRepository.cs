using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository(CityInfoContext context) : ICityInfoRepository
{
    private readonly CityInfoContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(city => city.Name).ToListAsync();
    }

    public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
    {
        if (includePointsOfInterest)
        {
            return await _context.Cities
                .Include(city => city.PointsOfInterest)
                .Where(city => city.Id == cityId)
                .FirstOrDefaultAsync();
        }
        else
        {
            return await _context.Cities
                .Where(city => city.Id == cityId)
                .FirstOrDefaultAsync();
        }
    }

    public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
    {
        return await _context.PointOfInterests
            .Where(point => point.CityId == cityId)
            .ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointOfInterests
            .Where(point => point.CityId == cityId && point.Id == pointOfInterestId)
            .FirstOrDefaultAsync();
    }
}