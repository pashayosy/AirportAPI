using System.Collections.ObjectModel;
using AirportAPI.Enums;
using AirportAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AirportAPI.Context;
public class AirportContext : DbContext
{
    public AirportContext(DbContextOptions<AirportContext> options) : base(options) { }

    public DbSet<Flight> Flights { get; set; }
    public DbSet<Leg> Legs { get; set; }
    public DbSet<FlightLog> FlightLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Leg>().ToTable("Legs");
        modelBuilder.Entity<Flight>().ToTable("Flights");
        modelBuilder.Entity<FlightLog>().ToTable("FlightLogs");

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.CurrentLeg)
            .WithMany(l => l.CurrectFlights)
            .HasForeignKey(f => f.LegId);

        modelBuilder.Entity<FlightLog>()
            .HasOne(fl => fl.Flight)
            .WithMany()
            .HasForeignKey(fl => fl.FlightId);

        modelBuilder.Entity<FlightLog>()
            .HasOne(fl => fl.Leg)
            .WithMany()
            .HasForeignKey(fl => fl.LegId);


        // Seed the Process with legs
        SeedWithData(modelBuilder);
    }

    public void SeedWithData(ModelBuilder modelBuilder)
    {
        Leg leg0 = new() { Id = -1, Name = "Wait for Landing", CrossingTime = 0, Capacity = int.MaxValue, Type = FlightStatus.None, CurrectFlights = { } };
        Leg leg1 = new() { Id = 1, Name = "Point 1", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Landing, CurrectFlights = { } };
        Leg leg2 = new() { Id = 2, Name = "Point 2", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Landing, CurrectFlights = { } };
        Leg leg3 = new() { Id = 3, Name = "Point 3", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Landing, CurrectFlights = { } };
        Leg leg4 = new() { Id = 4, Name = "Point 4", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Arrival | FlightStatus.Departure, CurrectFlights = { } };
        Leg leg5 = new() { Id = 5, Name = "Point 5", CrossingTime = 180, Capacity = 1, Type = FlightStatus.Arrival, CurrectFlights = { } };
        Leg leg6 = new() { Id = 6, Name = "Point 6", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Arrival, CurrectFlights = { } };
        Leg leg7 = new() { Id = 7, Name = "Point 7", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Arrival, CurrectFlights = { } };
        Leg leg8 = new() { Id = 8, Name = "Point 8", CrossingTime = 180, Capacity = 1, Type = FlightStatus.Departure, CurrectFlights = { } };
        Leg leg9 = new() { Id = 9, Name = "Point 9", CrossingTime = 60, Capacity = 1, Type = FlightStatus.Departure, CurrectFlights = { } };
        Leg leg10 = new() { Id = 10, Name = "Departure to other Place", CrossingTime = 0, Capacity = int.MaxValue, Type = FlightStatus.Departure, CurrectFlights = { } };


        modelBuilder.Entity<Leg>().HasData(leg0, leg1, leg2, leg3, leg4, leg5, leg6, leg7, leg8, leg9, leg10);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}

