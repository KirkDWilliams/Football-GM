using FootballGm.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Health check for connectivity from the Flutter client and local tooling.
    /// Reports whether the SQLite database is reachable.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthResponse>> Get(CancellationToken cancellationToken)
    {
        var databaseConnected = await _db.Database.CanConnectAsync(cancellationToken);
        var response = new HealthResponse(
            databaseConnected ? "healthy" : "degraded",
            DateTimeOffset.UtcNow,
            databaseConnected);

        if (!databaseConnected)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }

        return Ok(response);
    }
}

public record HealthResponse(string Status, DateTimeOffset Timestamp, bool DatabaseConnected);
