using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Domain;
using FootballGm.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ContractController : ControllerBase
{
    // User Requests:
    // Get the Contract for 'X'
    // Get the Contracts for Team 'A' !
    // Sign a new Contract for player 'X'!
    // Extend a Contract for player 'X'!
    // Terminate a Contract for player 'G'!

    // Service Actions
    // ----------------------
    // 1.0.  | Get team contracts
    // 2.0.  | Get player contract
    // 3.0.  | Sign new contract
    // 4.0.  | Extend contract
    // 5.0.  | Delete contract
    // 6.0.  | 
    // 7.0.  | 
    private readonly ContractOrchestrator _contractOrchestrator;

    public ContractController(ContractOrchestrator contractOrchestrator)
    {
        _contractOrchestrator = contractOrchestrator;
    }

    [HttpGet("{leagueId}/{teamId}")]
    public async Task<ActionResult<List<Data.Models.Contract>>> GetTeamContracts(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        CancellationToken cancellationToken)
    {
        if (leagueId == default || teamId == default)
            return BadRequest($"League {leagueId} or Team {teamId} must have valid values.");

        try
        {
            var contracts = await _contractOrchestrator.GetTeamContracts(leagueId, teamId, cancellationToken);

            if (contracts == null)
                return NotFound($"No contracts were found for Team {teamId}.");

            return Ok(contracts);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the contracts for team {teamId}.");
        }
    }

    [HttpGet("{leagueId}/{teamId}/{playerId}")]
    public async Task<ActionResult<Data.Models.Contract>> GetContract(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        [FromRoute] string playerId)
    {
        if (leagueId == default || teamId == default || string.IsNullOrWhiteSpace(playerId))
            return BadRequest($"League {leagueId}, Team {teamId}, or Player {playerId} must have valid values.");

        try
        {
            var contract = await _contractOrchestrator.GetContract(leagueId, teamId, playerId);

            if (contract == null)
                return NotFound($"No contract was found for Player {playerId} on Team {teamId}.");

            return Ok(contract);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while retrieving the contract for Player {playerId} on Team {teamId}.");
        }
    }

    [HttpPost("{leagueId}/{teamId}/{playerId}/Enact")]
    public async Task<ActionResult<bool>> Enact(
        [FromRoute] int leagueId,
        [FromRoute] int teamId,
        [FromRoute] string playerId,
        [FromBody] Data.Models.Contract contract,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _contractOrchestrator.CreateContract(leagueId, teamId, playerId, contract, cancellationToken);

            if (result is not true)
                return BadRequest($"Failed to enact a contract for Player {playerId} for Team {teamId}.");

            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while creating the contract for Player {playerId} on Team {teamId}.");
        }
    }

    [HttpPut("Extend")]
    public async Task<ActionResult<bool>> Extend(
        [FromBody] Data.Models.Contract contract,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _contractOrchestrator.ExtendContract(contract, cancellationToken);

            if (result is not true)
                return NotFound($"Either no contract was found for Contract {contract.ContractId}, or the update failed.");

            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while extending the contract {contract.ContractId}.");
        }
    }

    [HttpDelete("Drop")]
    public async Task<ActionResult<bool>> Drop(
        [FromBody] Data.Models.Contract contract,
        CancellationToken cancellationToken)
    {
        if (contract.EndWeek > WeekHelper.CurrentWeek || contract.ContractId == default)
            return BadRequest($"Contract {contract.ContractId} unable to be dropped.");

        try
        {
            var result = await _contractOrchestrator.DropContract(contract, cancellationToken);

            if (result is not true)
                NotFound($"Contract {contract.ContractId} was unsuccessfully eradicated.");

            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status408RequestTimeout, "Request was cancelled.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred when attempting to drop Contract {contract.ContractId}.");
        }
    }
}
