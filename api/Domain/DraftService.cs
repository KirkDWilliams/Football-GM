using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;

namespace FootballGm.Api.Domain;

public interface IDraftService
{
    Task<OpenDraftResult> Open(int leagueId, string userId);
    Task<JoinDraftResult> Join(int leagueId, string userId);
    Task Close(int leagueId);
    Task<DraftSnapshot?> GetSnapshot(int leagueId);
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

    public async Task<JoinDraftResult> Join(int leagueId, string userId)
    {
        var userMember = await leagueRepository
            .GetMembershipAsync(leagueId, userId);

        if (userMember == null)
            return new JoinDraftResult(JoinDraftStatus.UserNotInLeague);

        return new JoinDraftResult(JoinDraftStatus.Success, await GetSnapshot(leagueId));
    }

    public Task Close(int leagueId)
    {
        throw new NotImplementedException();
    }

    public async Task<DraftSnapshot?> GetSnapshot(int leagueId)
    {
        var draft = await draftRepository.GetLatestAsync(leagueId);
        return draft is null ? null : await BuildSnapshot(draft);
    }

    private async Task<OpenDraftResult> AddDraft(int leagueId)
    {
        var draft = await draftRepository.AddAsync(Draft.ToEntity(leagueId, DraftStatus.Lobby));
        return new OpenDraftResult(OpenDraftStatus.Success, await BuildSnapshot(draft));
    }

    private async Task<DraftSnapshot> BuildSnapshot(Data.Entity.Contrived.Draft draft)
    {
        var members = await leagueRepository.ListMembersAsync(draft.LeagueId);
        return DraftSnapshot.From(draft, members);
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

public enum JoinDraftStatus
{
    UserNotInLeague,
    Success
}

public sealed record OpenDraftResult(OpenDraftStatus Status, DraftSnapshot? Snapshot = null);

public sealed record JoinDraftResult(JoinDraftStatus Status, DraftSnapshot? Snapshot = null);
