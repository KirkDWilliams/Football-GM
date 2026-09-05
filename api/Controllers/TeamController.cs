using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class TeamController(
    ITeamOrchestrator teamOrchestrator,
    IContractOrchestrator contractOrchestrator) : ControllerBase
{
    [HttpGet("{teamId}")]
    public async Task<ActionResult<Data.Models.Budget>> GetBudget(
        [FromRoute] int teamId,
        CancellationToken cancellationToken)
    {
        try
        {
            var budget = await teamOrchestrator.GetBudget(teamId, cancellationToken);

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
            var updated = await teamOrchestrator.UpdateBudget(newBudget, cancellationToken);

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


    [HttpPost("{leagueId}")]
    public async Task<ActionResult<bool>> CreateTeam(
        [FromRoute] int leagueId,
        [FromBody] DraftOutcome draftOutcome,
        CancellationToken cancellationToken)
    {
        try
        {
            var team = await teamOrchestrator.CreateTeamInLeague(leagueId, draftOutcome, cancellationToken);
            var contracts = await contractOrchestrator.CreateContractsForTeam(leagueId, team, draftOutcome, cancellationToken);
            var paymentSchedule = BudgetHelper.CreatePaymentSchedule(contracts, Data.Enums.ContractType.Standard);
            var savedBudget = await teamOrchestrator.UpdateBudget(new Budget { TeamId = team.TeamId,  PaymentSchedule = paymentSchedule }, cancellationToken);
            // Add the TeamPlayerAssociations
               // return NotFound($"Error occured while creating the teams for League: {leagueId}");

            return Ok(true);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the Teams");
        }
    }
}
