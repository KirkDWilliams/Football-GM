using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public interface IDraftService
{
    Task<OpenDraftResult> Open(int leagueId, string userId);
    Task Join(int leagueId);
    Task Close(int leagueId);
}

public class DraftService(
    IDraftRepository draftRepository,
    ILeagueRepository leagueRepository) : IDraftService
{
    public async Task<OpenDraftResult> Open(int leagueId, string userId)
    {
        var userMember = await leagueRepository
            .GetMembershipAsync(leagueId, userId);

        if (userMember == null)
            return new OpenDraftResult(OpenDraftStatus.UserNotInLeague);

        if (userMember.Role != LeagueMemberRole.Commissioner)
            return new OpenDraftResult(OpenDraftStatus.NotCommissioner);

        var draft = await draftRepository.GetAsync(leagueId);

        return draft?.Status switch
        {
            DraftStatus.Lobby or DraftStatus.Live => new OpenDraftResult(OpenDraftStatus.ActiveDraft),
            DraftStatus.Complete => new OpenDraftResult(OpenDraftStatus.LeagueDraftCompleted),
            _ => await AddDraft(leagueId)
        };
    }

    public Task Join(int leagueId)
    {
        // Validate - They are a member of that league
        // Validate - There is an open draft to join

        // Add player to league members

        throw new NotImplementedException();
    }

    public Task Close(int leagueId)
    {
        throw new NotImplementedException();
    }

    private async Task<OpenDraftResult> AddDraft(int leagueId)
    {
        var draft = await draftRepository
            .AddAsync(Draft.ToEntity(leagueId, DraftStatus.Lobby));
        var members = await leagueRepository
            .ListMembersAsync(leagueId);

        return new OpenDraftResult(
            OpenDraftStatus.Success,
            DraftSnapshot.From(draft, members));
    }
}

public enum OpenDraftStatus
{
    UserNotInLeague,
    NotCommissioner,
    ActiveDraft,
    LeagueDraftCompleted,
    Success
}

public sealed record OpenDraftResult(OpenDraftStatus Status, DraftSnapshot? Snapshot = null);
