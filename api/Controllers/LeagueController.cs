using System.Security.Claims;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LeagueController(ILeagueOrchestrator orchestrator) : ControllerBase
{
    /// <summary>
    /// Create a league for the authenticated user. Omitting rules uses standard scoring weights.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(League), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<League>> CreateLeague(
        [FromBody] League league,
        CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(league.Name))
            return BadRequest(new { error = "Name is required." });

        var created = await orchestrator.CreateLeague(userId, league, cancellationToken);
        return Created($"/api/league/{created.LeagueId}", created);
    }
}
