using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportAPI.Models;

public class FlightLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int FlightId { get; set; }

    [ForeignKey("FlightId")]
    public virtual Flight? Flight { get; set; }

    public int LegId { get; set; }

    [ForeignKey("LegId")]
    public virtual Leg? Leg { get; set; }

    public DateTime In { get; set; }

    public DateTime? Out { get; set; }
}
