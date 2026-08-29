using FootballGm.Api.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballGm.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MatchController : ControllerBase
{
    public MatchController(){}

    // User Requests
    // Get the matchup between 'X' and 'Y'! ('Y' defaulting to the main user)
    // Get week 'Z' Matchups!
    // Get week combined Matchups!
    // 

    // Service Actions
    // 1.0.   Alter the Matchup between 'X' and 'Y' ('Y' defaulting to the main user)
    // 2.0.   Assign League Matchups. (Defined by league rules, typically set and unchanged, but perhaps dynamic per week)
    // 3.0.   Assess Match Outcomes.
    // 4.0.   Save Matchup 'Z' information.
    // 5.0.   Update Matchup 'W' information.
    // 6.0.   Assess Playoff Schedule.
    // 6.5.   Reveal results only to Teams in Matchup
    //          -and/or reveal the PlayOff Schedule only at a certain point.
    //          -
    // 6.6.   
    // 7.0.   Determine results in relation to the league average (above average, +1) (outliers rewarded-hindered)
    //          -or determine results in relation to player head- to- head matchups between players. (Ryder cup style)
    //          -or determine who had the best budget value for performance outcome (+1)
    //          -or weekly reseeding of playoffs to peg the highest scorer to the weakest scorer on playoff.
    //


    
}
