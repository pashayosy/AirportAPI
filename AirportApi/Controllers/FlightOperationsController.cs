using System.Text.Json;
using AirportAPI.Models;
using AirportAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirportAPI.Contrellers;

[Route("api/[controller]")]
[ApiController]
public class FlightOperationsController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightOperationsController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpPost("add-flight")]
    public async Task<IActionResult> AddFlight(Flight flight)
    {
        try
        {
            await _flightService.AddFlightAsync(flight);
            return CreatedAtAction(nameof(GetFlight), new { id = flight.Id }, flight);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("flight/{id}")]
    public async Task<ActionResult<Flight>> GetFlight(int id)
    {
        var flight = await _flightService.GetFlightAsync(id);
        if (flight == null)
        {
            return NotFound();
        }

        return Ok(flight);
    }

    [HttpGet("logs")]
    public async Task<ActionResult<IEnumerable<FlightLog>>> GetFlightLogs()
    {
        var logs = await _flightService.GetLogsAsync();
        return Ok(logs);
    }

    [HttpGet("flights")]
    public async Task<ActionResult<IEnumerable<FlightLog>>> GetFlights()
    {
        var logs = await _flightService.GetFlightsAsync();
        return Ok(logs);
    }
}
