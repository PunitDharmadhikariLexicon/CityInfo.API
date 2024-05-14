using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository(CityInfoContext context) : ICityInfoRepository
{
    private readonly CityInfoContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.OrderBy(city => city.Name).ToListAsync();
    }
    
    public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
        string? name,
        string? searchQuery,
        int pageNumber,
        int pageSize
    ) {
        var collection = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            collection = collection.Where(city => city.Name == name);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            searchQuery = searchQuery.Trim();
            collection = collection.Where(city =>
                city.Name.Contains(searchQuery) || (city.Description != null && city.Description.Contains(searchQuery)));
        }

        var totalItemCount = await collection.CountAsync();

        var paginationMetadata = new PaginationMetadata(
            totalItemCount, pageSize, pageNumber);
        

        var result = await collection
            .OrderBy(city => city.Name)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (result, paginationMetadata);
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
        return await _context.PointsOfInterest
            .Where(point => point.CityId == cityId)
            .ToListAsync();
    }

    public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
    {
        return await _context.PointsOfInterest
            .Where(point => point.CityId == cityId && point.Id == pointOfInterestId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CityExistsAsync(int cityId)
    {
        return await _context.Cities.AnyAsync(city => city.Id == cityId);
    }

    public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
    {
        var city = await GetCityAsync(cityId, false);
        city?.PointsOfInterest.Add(pointOfInterest);
    }

    public void DeletePointOfInterest(PointOfInterest pointOfInterest)
    {
        _context.PointsOfInterest.Remove(pointOfInterest);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }
}