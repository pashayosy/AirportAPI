using AirportAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAPI.Models;
public class Leg
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Capacity { get; set; }

    public FlightStatus? Type { get; set; }

    public int CrossingTime { get; set; } // in seconds

    public virtual ICollection<Flight>? CurrectFlights { get; set; }
}