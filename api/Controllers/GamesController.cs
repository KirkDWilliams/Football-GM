using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

/// <summary>
/// Game (matchup) resource. Named "Games" to align with the Flutter client domain model.
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    // Endpoints will be added when game features are implemented.
}
