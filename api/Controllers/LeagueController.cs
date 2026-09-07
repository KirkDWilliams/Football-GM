using System.Security.Claims;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;
using FootballGm.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LeagueController(ILeagueOrchestrator orchestrator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LeagueSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<LeagueSummary>>> GetMyLeagues(
        CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var leagues = await orchestrator.GetMyLeagues(userId, cancellationToken);
        return Ok(leagues);
    }

    [HttpGet("{leagueId:int}")]
    [ProducesResponseType(typeof(LeagueDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeagueDetails>> GetLeague(
        int leagueId,
        CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await orchestrator.GetLeague(userId, leagueId, cancellationToken);

        return result.Status switch
        {
            GetLeagueStatus.NotFound => NotFound(),
            GetLeagueStatus.Found => Ok(result.Details),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status, null)
        };
    }

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

        if (league.WeeklyCapSpace <= 0)
            return BadRequest(new { error = "Weekly cap must be greater than 0." });

        var created = await orchestrator.CreateLeague(userId, league, cancellationToken);
        return Created($"/api/league/{created.LeagueId}", created);
    }

    [HttpPost("{leagueCode}")]
    [ProducesResponseType(typeof(League), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<League>> JoinLeague(
        string leagueCode,
        CancellationToken cancellationToken)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        if (string.IsNullOrWhiteSpace(leagueCode))
            return BadRequest(new { error = "Join code is required." });

        var result = await orchestrator.JoinLeague(userId, leagueCode.Trim(), cancellationToken);

        return result.Status switch
        {
            JoinLeagueStatus.NotFound => NotFound(),
            JoinLeagueStatus.AlreadyMember => Conflict(result.League),
            JoinLeagueStatus.Joined => Created($"/api/league/{result.League!.LeagueId}", result.League),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Status), result.Status, null)
        };
    }
}
