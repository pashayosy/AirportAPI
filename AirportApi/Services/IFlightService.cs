using AirportAPI.Models;

namespace AirportAPI.Services;

public interface IFlightService
{
    Task AddFlightAsync(Flight flight);
    Task<Flight> GetFlightAsync(int flightId);
    Task<IEnumerable<FlightLog>> GetLogsAsync();
    Task<IEnumerable<Flight>> GetFlightsAsync();
    Task<IEnumerable<Leg>> GetLegsAsync();
}
