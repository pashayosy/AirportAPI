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
}
