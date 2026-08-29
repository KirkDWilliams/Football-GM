using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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

    [HttpGet("{id}")]
    public async Task<ActionResult<Data.Models.Budget>> GetBudget(int teamId, CancellationToken cancellationToken)
    {
        var budget = await _teamOrchestrator.GetBudget(teamId, cancellationToken);

        return Ok(budget);
    }

    [HttpPut("{id}")]
    public void UpdateBudget(int id, [FromBody] string value)
    {
    }
}
