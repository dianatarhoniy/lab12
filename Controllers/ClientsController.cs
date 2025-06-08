using Microsoft.AspNetCore.Mvc;
using Tutorial12.Services;

namespace Tutorial12.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly ITripService _service;

    public ClientsController(ITripService service)
    {
        _service = service;
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var result = await _service.DeleteClientAsync(idClient);
        if (!result)
            return BadRequest(new { error = "Client either not found or has assigned trips." });

        return Ok(new { message = "Client deleted successfully." });
    }
}