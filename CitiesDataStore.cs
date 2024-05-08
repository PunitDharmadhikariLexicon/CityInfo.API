using CityInfo.API.Models;

namespace CityInfo.API;

public class CitiesDataStore
{
    public List<CityDto> Cities { get; set; } =
    [
        new CityDto()
        {
            Id = 1,
            Name = "New York City",
            Description = "The one with the big park.",
            PointsOfInterest = new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id = 1,
                    Name = "Central Park",
                    Description = "The most visited urban park in the United States."
                },
                new PointOfInterestDto()
                {
                    Id = 2,
                    Name = "Empire State Building",
                    Description = "A 102-story skyscraper located in Midtown Manhattan."
                },
            }
        },

        new CityDto()
        {
            Id = 2,
            Name = "Melbourne",
            Description = "The one with the coffee.",
            PointsOfInterest = new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id = 1,
                    Name = "South Bank",
                    Description = "A river bank full of nice views and restaurants."
                },
                new PointOfInterestDto()
                {
                    Id = 2,
                    Name = "Great Ocean Road",
                    Description = "A long drive along the coast of Victoria."
                },
            }
        },

        new CityDto()
        {
            Id = 3,
            Name = "Paris",
            Description = "The one with that big tower.",
            PointsOfInterest = new List<PointOfInterestDto>()
            {
                new PointOfInterestDto()
                {
                    Id = 1,
                    Name = "Eiffel Tower",
                    Description = "A wrought iron lattice tower on the Champ de Mars."
                },
                new PointOfInterestDto()
                {
                    Id = 2,
                    Name = "The Louvre",
                    Description = "The world's largest museum."
                },
            }
        }
    ];

    public static CitiesDataStore Current { get; } = new CitiesDataStore();
}