using System.Security.Claims;
using FootballGm.Api.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FootballGm.Api.Hubs;

[Authorize]
public class DraftHub(IDraftService service) : Hub
{
    public async Task Open(int leagueId)
    {
        var userId =
            Context.UserIdentifier
            ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            throw new HubException("Unauthorized");

        var result = await service.Open(leagueId, userId);
        if (result.Snapshot is null || result.Status != OpenDraftStatus.Success)
            throw new HubException(result.Status.ToString());

        await Clients
            .Group($"draft-{leagueId}")
            .SendAsync("DraftUpdated", result.Snapshot);
    }

    public async Task Join(int leagueId)
    {
        var userId =
            Context.UserIdentifier
            ?? Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(userId))
            throw new HubException("Unauthorized");

        var result = await service.Join(leagueId, userId);
        if (result.Status != JoinDraftStatus.Success)
            throw new HubException(result.Status.ToString());

        await Groups.AddToGroupAsync(Context.ConnectionId, $"draft-{leagueId}");

        if (result.Snapshot is not null)
            await Clients.Caller.SendAsync("DraftUpdated", result.Snapshot);
    }

    public async Task Close(int leagueId)
    {
        var connectionId = Context.ConnectionId;
    }
}
