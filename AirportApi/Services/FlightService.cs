using AirportAPI.Context;
using Microsoft.EntityFrameworkCore;
using AirportAPI.Models;
using AirportAPI.Enums;

namespace AirportAPI.Services
{
    public class FlightService : IFlightService
    {
        private readonly AirportContext _context;

        public FlightService(AirportContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync()
        {
            return await _context.Flights.Include(f => f.CurrentLeg).ToListAsync();
        }

        public async Task AddFlightAsync(Flight flight)
        {
            await _context.Flights.AddAsync(flight);
            await _context.SaveChangesAsync();
            await AddFlightToLeg(-1, flight.Id);
        }

        public async Task<Flight> GetFlightAsync(int flightId)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);
            return flight;
        }

        public async Task<IEnumerable<FlightLog>> GetLogsAsync()
        {
            return await _context.FlightLogs.Include(log => log.Flight).Include(log => log.Leg).ToListAsync();
        }

        public async Task<IEnumerable<Leg>> GetLegsAsync()
        {
            return await _context.Legs.Include(l => l.CurrectFlights).ToListAsync();
        }
        public async Task AddFlightToLeg(int legId, int flightId)
        {
            var leg = await _context.Legs.FirstOrDefaultAsync(l => l.Id == legId);
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);

            if (leg == null || flight == null)
            {
                throw new ArgumentException("Leg or Flight not found.");
            }

            if (leg.Id == 4)
            {
                flight.Status = flight.Status == FlightStatus.Landing ? FlightStatus.Arrival : FlightStatus.Departure;
            }
            else
            {
                flight.Status = leg.Type;
            }

            flight.LegId = legId;
            leg.CurrectFlights.Add(flight);

            var log = await _context.FlightLogs.FirstOrDefaultAsync(log => log.FlightId == flight.Id && log.LegId == legId);

            if (log == null)
            {
                log = new FlightLog
                {
                    FlightId = flight.Id,
                    LegId = leg.Id,
                    In = DateTime.UtcNow
                };
                await _context.FlightLogs.AddAsync(log);
            }
            else
            {
                log.In = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
