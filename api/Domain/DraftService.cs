using FootballGm.Api.Data.Enums;
using FootballGm.Api.Data.Models;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;
using DraftEntity = FootballGm.Api.Data.Entity.Contrived.Draft;

namespace FootballGm.Api.Domain;

public interface IDraftService
{
    /// <summary>
    /// Commissioner only. Creates a lobby when the league has no in-play draft.
    /// A closed draft is not in play, so a new lobby can be opened after Close.
    /// An existing lobby or live draft is <see cref="OpenDraftStatus.ActiveDraft"/>.
    /// A completed draft is <see cref="OpenDraftStatus.LeagueDraftCompleted"/>.
    /// A lost insert race returns that same result.
    /// </summary>
    Task<OpenDraftResult> Open(int leagueId, string userId);

    /// <summary>
    /// Admits a league member. The snapshot is the latest draft, including a closed
    /// one, or null when no draft exists yet so the member can wait for Open.
    /// </summary>
    Task<JoinDraftResult> Join(int leagueId, string userId);

    /// <summary>
    /// Commissioner only. The in-play draft must be a lobby. Marks it live, freezes
    /// nomination order by join time then user id, and names the first member as nominator.
    /// No in-play draft is <see cref="StartDraftStatus.LobbyNotStarted"/>.
    /// A live or completed draft is <see cref="StartDraftStatus.NotInLobby"/>.
    /// </summary>
    Task<StartDraftResult> Start(int leagueId, string userId);

    /// <summary>
    /// Commissioner only. Closes the in-play lobby. No in-play draft, including a
    /// league whose latest draft is already closed, is <see cref="CloseDraftStatus.NoDraftFound"/>.
    /// A live or completed draft is <see cref="CloseDraftStatus.DraftInactive"/>.
    /// </summary>
    Task<CloseDraftResult> Close(int leagueId, string userId);
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

        var draft = await InPlayDraft(leagueId);
        if (draft is null)
            return await AddDraft(leagueId);

        return ResultForExisting(draft);
    }

    public async Task<JoinDraftResult> Join(int leagueId, string userId)
    {
        var userMember = await leagueRepository
            .GetMembershipAsync(leagueId, userId);

        if (userMember == null)
            return new JoinDraftResult(JoinDraftStatus.UserNotInLeague);

        return new JoinDraftResult(JoinDraftStatus.Success, await LatestSnapshot(leagueId));
    }

    public async Task<StartDraftResult> Start(int leagueId, string userId)
    {
        var userMember = await leagueRepository
            .GetMembershipAsync(leagueId, userId);

        if (userMember == null)
            return new StartDraftResult(StartDraftStatus.UserNotInLeague);

        if (userMember.Role != LeagueMemberRole.Commissioner)
            return new StartDraftResult(StartDraftStatus.NotCommissioner);

        var draft = await InPlayDraft(leagueId);
        if (draft is null)
            return new StartDraftResult(StartDraftStatus.LobbyNotStarted);
        if (draft.Status != DraftStatus.Lobby)
            return new StartDraftResult(StartDraftStatus.NotInLobby);

        var members = await leagueRepository.ListMembersAsync(draft.LeagueId);
        var nominationOrder = DraftSnapshot.OrderedByJoin(members)
            .Select(member => member.UserId)
            .ToList();
        draft.Status = DraftStatus.Live;
        draft.NominationOrder = nominationOrder;
        draft.CurrentNominatorUserId = nominationOrder[0];
        await draftRepository.UpdateAsync(draft);

        return new StartDraftResult(
            StartDraftStatus.Success,
            DraftSnapshot.From(draft, members));
    }

    public async Task<CloseDraftResult> Close(int leagueId, string userId)
    {
        var userMember = await leagueRepository
            .GetMembershipAsync(leagueId, userId);

        if (userMember == null)
            return new CloseDraftResult(CloseDraftStatus.UserNotInLeague);

        if (userMember.Role != LeagueMemberRole.Commissioner)
            return new CloseDraftResult(CloseDraftStatus.NotCommissioner);

        var draft = await InPlayDraft(leagueId);
        if (draft is null)
            return new CloseDraftResult(CloseDraftStatus.NoDraftFound);

        if (draft.Status != DraftStatus.Lobby)
            return new CloseDraftResult(CloseDraftStatus.DraftInactive);

        return await CloseLobby(draft);
    }

    /// <summary>
    /// The draft Open, Start, and Close can still change. Closed rows are excluded.
    /// </summary>
    private Task<DraftEntity?> InPlayDraft(int leagueId) =>
        draftRepository.GetAsync(leagueId);

    /// <summary>
    /// Newest draft, including a closed one. Null when the league has no draft yet.
    /// </summary>
    private async Task<DraftSnapshot?> LatestSnapshot(int leagueId)
    {
        var draft = await draftRepository.GetLatestAsync(leagueId);
        return draft is null ? null : await BuildSnapshot(draft);
    }

    private async Task<OpenDraftResult> AddDraft(int leagueId)
    {
        try
        {
            var draft = await draftRepository.AddAsync(Draft.ToEntity(leagueId, DraftStatus.Lobby));
            return new OpenDraftResult(OpenDraftStatus.Success, await BuildSnapshot(draft));
        }
        catch (DraftAlreadyInPlayException)
        {
            return ResultForExisting(await InPlayDraft(leagueId));
        }
    }

    /// <summary>
    /// Lobby and live are already open. Complete is finished. A missing row here
    /// means the insert lost the race and the winner is no longer in play.
    /// </summary>
    private static OpenDraftResult ResultForExisting(DraftEntity? draft) =>
        draft?.Status switch
        {
            DraftStatus.Complete => new OpenDraftResult(OpenDraftStatus.LeagueDraftCompleted),
            _ => new OpenDraftResult(OpenDraftStatus.ActiveDraft)
        };

    private async Task<CloseDraftResult> CloseLobby(DraftEntity lobby)
    {
        lobby.Status = DraftStatus.Closed;
        await draftRepository.UpdateAsync(lobby);
        return new CloseDraftResult(CloseDraftStatus.Success, await BuildSnapshot(lobby));
    }

    private async Task<DraftSnapshot> BuildSnapshot(DraftEntity draft)
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

public enum StartDraftStatus
{
    UserNotInLeague,
    NotCommissioner,
    LobbyNotStarted,
    NotInLobby,
    Success
}

public enum CloseDraftStatus
{
    UserNotInLeague,
    NotCommissioner,
    NoDraftFound,
    DraftInactive,
    Success
}

public sealed record OpenDraftResult(OpenDraftStatus Status, DraftSnapshot? Snapshot = null);
public sealed record JoinDraftResult(JoinDraftStatus Status, DraftSnapshot? Snapshot = null);
public sealed record StartDraftResult(StartDraftStatus Status, DraftSnapshot? Snapshot = null);
public sealed record CloseDraftResult(CloseDraftStatus Status, DraftSnapshot? Snapshot = null);
