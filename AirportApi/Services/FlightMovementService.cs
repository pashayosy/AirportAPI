using AirportAPI.Context;
using Microsoft.EntityFrameworkCore;
using AirportAPI.Models;
using AirportAPI.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirportAPI.Services
{
    public class FlightMovementService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FlightMovementService> _logger;
        private readonly ConcurrentQueue<Flight> _flightQueue = new ConcurrentQueue<Flight>();

        public FlightMovementService(IServiceScopeFactory scopeFactory, ILogger<FlightMovementService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FlightMovementService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AirportContext>();
                        var flights = await context.Flights.Include(f => f.CurrentLeg).ToListAsync();

                        foreach (var flight in flights.Where(f => f.CurrentLeg.Id != 10 && f.CurrentLeg.Id != -1).OrderByDescending(f => f.CurrentLeg.Id))
                        {
                            _flightQueue.Enqueue(flight);
                        }
                        var getWatingFlight = flights.FirstOrDefault(f => f.CurrentLeg.Id == -1);
                        if (getWatingFlight != null)
                        {
                            _flightQueue.Enqueue(getWatingFlight);
                        }
                    }
                    List<Task> tasks = new List<Task>();

                    while (_flightQueue.TryDequeue(out var flight))
                    {
                        // Run Tasl after task async and save to task list
                        tasks.Add(Task.Run(async () =>
                        {
                            await MoveFlightAsync(flight, stoppingToken);
                        }));
                        await Task.Delay(500);
                    }
                    // Wait until all the Task finished
                    await Task.WhenAll(tasks);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in FlightMovementService.");
                }

                await Task.Delay(1000, stoppingToken); // Check every second
            }

            _logger.LogInformation("FlightMovementService stopped.");
        }

        private async Task MoveFlightAsync(Flight flight, CancellationToken stoppingToken)
        {
            // Create a scope for the flight
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AirportContext>();

                if (flight.CurrentLeg == null || flight.CurrentLeg.Id == 10) return;

                var legId = flight.CurrentLeg.Id;
                try
                {
                    bool respond = false;
                    _logger.LogInformation($"Attempting to move flight {flight.Id} from leg {legId}.");

                    switch (legId)
                    {
                        case -1:
                            await WaitAndMoveFlight(context, flight, 1, stoppingToken);
                            break;
                        case 3:
                        case 8:
                            respond = await PriorityWaitFor(context, 4, 8, flight.Status, stoppingToken);
                            if (respond)
                                await WaitAndMoveFlight(context, flight, 4, stoppingToken);
                            break;
                        case 5:
                            var freeLegId = await WaitAtlistOneFree(context, stoppingToken, 6, 7);
                            if (freeLegId != -2)
                                await WaitAndMoveFlight(context, flight, freeLegId, stoppingToken);
                            break;
                        case 6:
                            await WaitAndMoveFlight(context, flight, 8, stoppingToken);
                            break;
                        case 7:
                            await WaitAndMoveFlight(context, flight, 8, stoppingToken);
                            break;
                        case 9:
                            await WaitAndMoveFlight(context, flight, 10, stoppingToken);
                            break;
                        case 4:
                            if (flight.Status == FlightStatus.Departure)
                            {
                                await WaitAndMoveFlight(context, flight, 9, stoppingToken);
                            }
                            else
                            {
                                await WaitAndMoveFlight(context, flight, 5, stoppingToken);
                            }
                            break;
                        default:
                            await WaitAndMoveFlight(context, flight, legId + 1, stoppingToken);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while moving flight with Id {flight.Id}.");
                }
            }
        }

        private async Task WaitAndMoveFlight(AirportContext context, Flight flight, int nextLegId, CancellationToken stoppingToken)
        {
            if (!await WaitLegFree(context, nextLegId, stoppingToken))
            {
                _logger.LogInformation($"Flight {flight.Id} waiting for leg {nextLegId} to be free.");
                return;
            }

            await MoveFlightToNextLeg(context, flight, flight.CurrentLeg.Id, nextLegId, stoppingToken);
            return;
        }

        private async Task MoveFlightToNextLeg(AirportContext context, Flight flight, int currentLegId, int nextLegId, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Moving flight {flight.Id} from leg {currentLegId} to leg {nextLegId}.");
                await RemoveFlightFromLeg(context, currentLegId, flight, stoppingToken);
                await AddFlightToLeg(context, nextLegId, flight.Id, stoppingToken);
                await PassTheLeg(context, nextLegId, stoppingToken);
                _logger.LogInformation($"Flight {flight.Id} successfully moved from leg {currentLegId} to leg {nextLegId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while moving flight with Id {flight.Id} from leg {currentLegId} to leg {nextLegId}.");
            }
        }

        private async Task RemoveFlightFromLeg(AirportContext context, int legId, Flight flight, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Removing flight {flight.Id} from leg {legId}.");
                var leg = await context.Legs.FirstOrDefaultAsync(leg => leg.Id == legId);
                if (leg != null)
                {
                    leg.CurrectFlights.Remove(flight);
                    _logger.LogInformation($"Leg {legId} after removal: {leg.CurrectFlights.Count}/{leg.Capacity} flights.");
                }

                flight.CurrentLeg = null;

                var log = await context.FlightLogs.FirstOrDefaultAsync(log => log.FlightId == flight.Id && log.LegId == legId);
                if (log != null)
                {
                    log.Out = DateTime.UtcNow;
                }

                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Flight {flight.Id} removed from leg {legId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while removing flight with Id {flight.Id} from leg {legId}.");
            }
        }

        private async Task AddFlightToLeg(AirportContext context, int legId, int flightId, CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation($"Adding flight {flightId} to leg {legId}.");
                var leg = await context.Legs.FirstOrDefaultAsync(l => l.Id == legId);
                var flight = await context.Flights.FirstOrDefaultAsync(f => f.Id == flightId);

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

                var log = await context.FlightLogs.FirstOrDefaultAsync(log => log.FlightId == flight.Id && log.LegId == legId);

                if (log == null)
                {
                    log = new FlightLog
                    {
                        FlightId = flight.Id,
                        LegId = leg.Id,
                        In = DateTime.UtcNow
                    };
                    await context.FlightLogs.AddAsync(log, stoppingToken);
                }
                else
                {
                    log.In = DateTime.UtcNow;
                }

                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation($"Leg {legId} after adding flight: {leg.CurrectFlights.Count}/{leg.Capacity} flights.");
                _logger.LogInformation($"Flight {flightId} added to leg {legId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while adding flight with Id {flightId} to leg {legId}.");
            }
        }

        private async Task<int> WaitAtlistOneFree(AirportContext context, CancellationToken stoppingToken, params int[] legIds)
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    foreach (int legId in legIds)
                    {
                        var leg = await context.Legs.FirstOrDefaultAsync(leg => leg.Id == legId);
                        if (leg == null)
                        {
                            throw new ArgumentException($"Leg with Id {legId} not found.");
                        }

                        _logger.LogInformation($"Checking leg {legId}: {leg.CurrectFlights.Count}/{leg.Capacity} flights.");

                        if (leg.Capacity - leg.CurrectFlights.Count > 0)
                        {
                            _logger.LogInformation($"Found free leg {legId} for flight.");
                            return legId;
                        }
                    }

                    _logger.LogInformation("No free leg found. Waiting for a second before checking again.");
                    return -2;
                }

                throw new OperationCanceledException(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while waiting for at least one free leg.");
                throw;
            }
        }

        private async Task<bool> PriorityWaitFor(AirportContext context, int legId, int lastPriorityPlace, FlightStatus? status, CancellationToken stoppingToken)
        {
            try
            {
                bool shouldWait = true;

                if (shouldWait && !stoppingToken.IsCancellationRequested)
                {
                    var leg = await context.Legs.FirstOrDefaultAsync(leg => leg.Id == legId);

                    if (leg == null)
                    {
                        throw new ArgumentException($"Leg with Id {legId} not found.");
                    }

                    IList<Leg> legArray = await context.Legs.ToListAsync();
                    IList<Leg> legCircle = legArray.Where(l => legId <= l.Id && l.Id <= lastPriorityPlace).ToList();
                    int countFlightAmount = legCircle.Sum(l => l.CurrectFlights.Count);

                    if (status == FlightStatus.Landing && legCircle.Count - 1 > countFlightAmount)
                    {
                        _logger.LogInformation($"Leg {legId} Priority for Landing.");
                        return true;
                    }

                    if (status == FlightStatus.Departure && legCircle.Count - 1 <= countFlightAmount || legArray.FirstOrDefault(l => l.Id == 3)?.CurrectFlights.Count <= 0)
                    {
                        _logger.LogInformation($"Leg {legId} Priority for Departure.");
                        return true;
                    }
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during priority wait.");
                throw;
            }
            return false;
        }

        private async Task PassTheLeg(AirportContext context, int legId, CancellationToken stoppingToken)
        {
            try
            {
                var leg = await context.Legs.FirstOrDefaultAsync(leg => leg.Id == legId);

                if (leg != null)
                {
                    _logger.LogInformation($"Passing the leg {legId}. Waiting for {10 * leg.CrossingTime} milliseconds.");
                    await Task.Delay(10 * leg.CrossingTime, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while passing the leg with Id {legId}.");
                throw;
            }
        }

        private async Task<bool> WaitLegFree(AirportContext context, int legId, CancellationToken stoppingToken)
        {
            try
            {
                var leg = await context.Legs.FirstOrDefaultAsync(leg => leg.Id == legId);

                if (leg == null)
                {
                    throw new ArgumentException($"Leg with Id {legId} not found.");
                }

                _logger.LogInformation($"Checking leg {legId}: {leg.CurrectFlights.Count}/{leg.Capacity} flights.");

                return leg.Capacity - leg.CurrectFlights.Count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while waiting for leg {legId} to be free.");
                return false;
            }
        }
    }
}
