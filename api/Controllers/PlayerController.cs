using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlayerController(IPlayerOrchestrator orchestrator) : ControllerBase
{
    /// <summary>
    /// Get a player. Omit <c>stats</c> for identity only; pass one or more stat sets
    /// (<c>PreviousWeek</c>, <c>Season</c>, <c>RecentThreeGames</c>) to include scores.
    /// LeagueId is required when requesting stats. GameId is required for PreviousWeek.
    /// </summary>
    [HttpGet("{playerId}")]
    [ProducesResponseType(typeof(Player), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Player>> GetPlayer(
        string playerId,
        [FromQuery] StatSetKind[]? stats,
        [FromQuery] int? leagueId,
        [FromQuery] string? gameId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(playerId))
            return BadRequest(new { error = "PlayerId is required." });

        var requested = stats ?? [];

        if (requested.Length > 0 && leagueId is null or <= 0)
            return BadRequest(new { error = "LeagueId is required when requesting stats." });

        if (requested.Contains(StatSetKind.PreviousWeek) && string.IsNullOrWhiteSpace(gameId))
            return BadRequest(new { error = "GameId is required when requesting previous week stats." });

        try
        {
            var player = await orchestrator.GetPlayer(
                playerId,
                requested,
                leagueId,
                gameId,
                cancellationToken);
            return Ok(player);
        }
        catch (ArgumentNullException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
