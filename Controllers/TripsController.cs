using Microsoft.AspNetCore.Mvc;
using Tutorial12.DTOs;
using Tutorial12.Services;

namespace Tutorial12.Controllers;

[ApiController]
[Route("api/trips")]
public class TripsController : ControllerBase
{
    private readonly ITripService _service;

    public TripsController(ITripService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetTripsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] AddClientToTripRequest request)
    {
        var message = await _service.AddClientToTripAsync(idTrip, request);
        if (message == "Client registered successfully.")
            return Ok(new { message });

        return BadRequest(new { error = message });
    }
}