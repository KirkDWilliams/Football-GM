using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FootballGm.Api.Hubs;

[Authorize]
public class DraftHub : Hub
{
    public async Task Join(string draftId)
    {
        var connectionId = Context.ConnectionId;
        var groupName = "draft-" + draftId;

        await Groups.AddToGroupAsync(connectionId, groupName);
        //await Clients.Caller.SendAsync("CounterChanged", Values.GetOrAdd(groupName, 0));
    }
}
