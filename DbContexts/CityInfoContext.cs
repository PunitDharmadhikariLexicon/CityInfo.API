using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts;

public class CityInfoContext(DbContextOptions<CityInfoContext> options) : DbContext(options)
{
    public DbSet<City> Cities { get; set; }
    
    public DbSet<PointOfInterest> PointOfInterests { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("connectionstring");
    //     
    //     base.OnConfiguring(optionsBuilder);
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new City("New York City")
            {
                Id = 1,
                Description = "The one with the big park."
            },
            new City("Melbourne")
            {
                Id = 2,
                Description = "The one with the river bank."
            },
            new City("Paris")
            {
                Id = 3,
                Description = "The one with the big tower."
            }
        );

        modelBuilder.Entity<PointOfInterest>().HasData(
            new PointOfInterest("Central Park")
            {
                Id = 1,
                CityId = 1,
                Description = "The most visited urban park in the United States."
            },
            new PointOfInterest("Empire State Building")
            {
                Id = 2,
                CityId = 1,
                Description = "The 102-storey skyscraper located in Midtown Manhattan."
            },
            new PointOfInterest("South Bank")
            {
                Id = 3,
                CityId = 2,
                Description = "A river bank."
            },
            new PointOfInterest("Eiffel Tower")
            {
                Id = 4,
                CityId = 3,
                Description = "A wrought iron lattice tower on the Champ de Mars."
            },
            new PointOfInterest("The Louvre")
            {
                Id = 5,
                CityId = 3,
                Description = "The world's largest museum."
            }
        );
        
        base.OnModelCreating(modelBuilder);
    }
}