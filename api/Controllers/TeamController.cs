using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TeamController : ControllerBase
{
    private readonly TeamOrchestrator _teamOrchestrator;
    public TeamController(TeamOrchestrator teamOrchestator)
    {
        _teamOrchestrator = teamOrchestator;
    }

    [HttpGet("{teamId}")]
    public async Task<ActionResult<Data.Models.Budget>> GetBudget(
        [FromRoute] int teamId,
        CancellationToken cancellationToken)
    {
        try
        {
            var budget = await _teamOrchestrator.GetBudget(teamId, cancellationToken);

            if (budget == null)
                return NotFound($"Budget not found for team {teamId}");

            return Ok(budget);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the budget");
        }
    }

    [HttpPut("{teamId}")]
    public async Task<ActionResult<bool>> UpdateBudget(
        [FromRoute] int teamId,
        [FromBody] Data.Models.Budget newBudget,
        CancellationToken cancellationToken)
    {
        if (newBudget == null)
            return BadRequest("Budget cannot be null");

        if (newBudget.TeamId != teamId)
            return BadRequest("Budget Team ID must match the URL parameter");

        try
        {
            var updated = await _teamOrchestrator.UpdateBudget(newBudget, cancellationToken);

            if (!updated)
                return NotFound($"Budget not found for team {teamId}");

            return Ok(updated);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the budget");
        }
    }
}
