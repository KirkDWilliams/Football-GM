using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Domain;
using FootballGm.Api.Infrastructure;
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
    public async Task<ActionResult<List<Data.Models.Contract>>> GetTeamContracts(int leagueId, int teamId, CancellationToken cancellationToken)
    {
        try
        {
            List<Data.Models.Contract> contracts = await _contractOrchestrator.GetTeamContracts(leagueId, teamId, cancellationToken);

            return Ok(contracts);
        }
        catch
        {
            return NotFound($"Team: {teamId} in " +
                          $"League: {leagueId} not found");
        }
    }

    /* ADMIN */
    /*[HttpGet("{leagueId}/{teamId}/{playerId}")]
    public async Task<ActionResult<Contract>> GetContract(int leagueId, int teamId, string playerId)
    {
        try
        {
            Contract contract = await _contractOrchestrator.GetContractAsync(leagueId, teamId, playerId);

            return Ok(contract);
        }
        catch
        {
            return NotFound($"Team {teamId} or Player {playerId} not found");
        }
    }*/

    [HttpPost("{leagueId}/{teamId}/{playerId}/Enact")]
    public async Task<ActionResult<bool>> Enact(int leagueId, int teamId, string playerId, [FromBody] Data.Models.Contract contract, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _contractOrchestrator.CreateContract(leagueId, teamId, playerId, contract, cancellationToken);
            return Ok(result);
        }
        catch
        {
            return BadRequest("Failed to sign contract");
        }
    }

    [HttpPut("Extend")]
    public async Task<ActionResult<bool>> Extend([FromBody] Data.Models.Contract contract, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _contractOrchestrator.ExtendContract(contract, cancellationToken);
            return Ok(result);
        }
        catch
        {
            return NotFound(false);
        }
    }

    [HttpDelete("Drop")]
    public async Task<ActionResult<bool>> Drop([FromBody] Data.Models.Contract contract, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _contractOrchestrator.DeleteContract(contract, cancellationToken);
            return Ok(result);
        }
        catch
        {
            return BadRequest(false);
        }
    }
}
