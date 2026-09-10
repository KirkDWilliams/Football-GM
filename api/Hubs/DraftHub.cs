using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FootballGm.Api.Hubs;

// PLAYGROUND: counter Join/Increment/Decrement. Keep the hub class; replace these methods with draft commands.
[Authorize]
public class DraftHub : Hub
{
    private static readonly ConcurrentDictionary<string, int> Values = new();

    public async Task Join(string draftId)
    {
        var connectionId = Context.ConnectionId;
        var groupName = "draft-" + draftId;

        await Groups.AddToGroupAsync(connectionId, groupName);
        await Clients.Caller.SendAsync("CounterChanged", Values.GetOrAdd(groupName, 0));
    }

    public async Task Increment(string draftId)
    {
        var groupName = "draft-" + draftId;
        var value = Values.AddOrUpdate(groupName, 1, (_, n) => n + 1);
        await Clients.Group(groupName).SendAsync("CounterChanged", value);
    }

    public async Task Decrement(string draftId)
    {
        var groupName = "draft-" + draftId;
        var value = Values.AddOrUpdate(groupName, -1, (_, n) => n - 1);
        await Clients.Group(groupName).SendAsync("CounterChanged", value);
    }
}
