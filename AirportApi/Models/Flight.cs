using AirportAPI.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirportAPI.Models;
public class Flight
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Number { get; set; }

    public int? PassengersCount { get; set; }
    public string? Brand { get; set; }
    public FlightStatus? Status { get; set; }

    public int? LegId { get; set; }

    [ForeignKey("LegId")]
    public virtual Leg? CurrentLeg { get; set; }

    // Delegate and event for flight movement
    public delegate Task FlightMovedHandler(Flight flight, CancellationToken stoppingToken);
    public FlightMovedHandler OnFlightMoved;

    public async Task MoveToNextLegAsync(CancellationToken stoppingToken)
    {
        if (OnFlightMoved != null)
        {
            await OnFlightMoved(this, stoppingToken);
        }
    }

    public async Task MoveFlightAsyncWrapper(CancellationToken stoppingToken)
    {
        while (CurrentLeg?.Id != 10)
        {
            if (OnFlightMoved != null)
            {
                await MoveToNextLegAsync(stoppingToken);
            }
        }
        OnFlightMoved = null;

    }
}
