using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain.Interfaces;
using FootballGm.Api.Services.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ContractController(IContractOrchestrator contractOrchestrator) : ControllerBase
{
    [HttpGet("{leagueId:int}/{teamId:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<Contract>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<Contract>>> GetTeamContracts(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        CancellationToken cancellationToken)
    {
        if (IsInvalidTeamRoute(leagueId, teamId))
            return BadRequest(new { error = $"League {leagueId} or Team {teamId} must have valid values." });

        var contracts = await contractOrchestrator.GetTeamContracts(leagueId, teamId, cancellationToken);
        return Ok(contracts);
    }

    [HttpGet("{leagueId:int}/{teamId:int}/{playerId}")]
    [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Contract>> GetContract(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        [FromRoute] string playerId,
        CancellationToken cancellationToken)
    {
        if (IsInvalidTeamRoute(leagueId, teamId) || string.IsNullOrWhiteSpace(playerId))
            return BadRequest(new { error = $"League {leagueId}, Team {teamId}, or Player {playerId} must have valid values." });

        var contract = await contractOrchestrator.GetContract(leagueId, teamId, playerId, cancellationToken);
        if (contract is null)
            return NotFound($"No contract was found for Player {playerId} on Team {teamId}.");

        return Ok(contract);
    }

    [HttpPost("{leagueId:int}/{teamId:int}/{playerId}/Create")]
    [ProducesResponseType(typeof(Contract), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Contract>> Create(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        [FromRoute] string playerId,
        [FromBody] Contract contract,
        CancellationToken cancellationToken)
    {
        if (IsInvalidTeamRoute(leagueId, teamId) || string.IsNullOrWhiteSpace(playerId))
            return BadRequest(new { error = $"League {leagueId}, Team {teamId}, or Player {playerId} must have valid values." });

        var created = await contractOrchestrator.CreateContract(
            leagueId,
            teamId,
            playerId,
            contract,
            cancellationToken);

        if (created is null)
            return BadRequest(new { error = $"Failed to enact a contract for Player {playerId} for Team {teamId}." });

        return Ok(created);
    }

    [HttpPut("Update")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> Update(
        [FromBody] Contract contract,
        CancellationToken cancellationToken)
    {
        var updated = await contractOrchestrator.UpdateContract(contract, cancellationToken);
        if (!updated)
            return NotFound($"Either no contract was found for Contract {contract.ContractId}, or the update failed.");

        return Ok(updated);
    }

    [HttpDelete("Drop")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<bool>> Drop(
        [FromBody] Contract contract,
        CancellationToken cancellationToken)
    {
        if (contract.EndWeek > WeekHelper.CurrentWeek || contract.ContractId <= 0)
            return BadRequest(new { error = $"Contract {contract.ContractId} unable to be dropped." });

        var dropped = await contractOrchestrator.DropContract(contract, cancellationToken);
        if (!dropped)
            return NotFound($"Contract {contract.ContractId} was unsuccessfully eradicated.");

        return Ok(dropped);
    }

    private static bool IsInvalidTeamRoute(int leagueId, int teamId) => leagueId <= 0 || teamId <= 0;
}
