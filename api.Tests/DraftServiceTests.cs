using FootballGm.Api.Data.Entity.Contrived;
using FootballGm.Api.Data.Enums;
using FootballGm.Api.Domain;
using FootballGm.Api.Infrastructure;
using FootballGm.Api.Infrastructure.Interfaces;
using DraftEntity = FootballGm.Api.Data.Entity.Contrived.Draft;

namespace FootballGm.Api.Tests;

public class DraftServiceTests
{
    private const int LeagueId = 7;
    private const string CommissionerId = "user-commish";
    private const string MemberId = "user-member";
    private const string StrangerId = "user-stranger";

    [Fact]
    public async Task Open_rejects_user_not_in_league()
    {
        var result = await Sut().Open(LeagueId, StrangerId);

        Assert.Equal(OpenDraftStatus.UserNotInLeague, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Open_rejects_non_commissioner()
    {
        var result = await Sut().Open(LeagueId, MemberId);

        Assert.Equal(OpenDraftStatus.NotCommissioner, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Open_creates_lobby_when_no_draft_exists()
    {
        var result = await Sut().Open(LeagueId, CommissionerId);

        Assert.Equal(OpenDraftStatus.Success, result.Status);
        Assert.NotNull(result.Snapshot);
        Assert.Equal(DraftStatus.Lobby, result.Snapshot.Status);
        Assert.Equal(LeagueId, result.Snapshot.LeagueId);
        Assert.Contains(result.Snapshot.Members, m => m.UserId == CommissionerId);
    }

    [Fact]
    public async Task Open_rejects_when_lobby_or_live_already_exists()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Lobby });

        var result = await Sut(drafts).Open(LeagueId, CommissionerId);

        Assert.Equal(OpenDraftStatus.ActiveDraft, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Open_rejects_when_league_draft_is_complete()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Complete });

        var result = await Sut(drafts).Open(LeagueId, CommissionerId);

        Assert.Equal(OpenDraftStatus.LeagueDraftCompleted, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Open_creates_new_lobby_after_close()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Closed });

        var result = await Sut(drafts).Open(LeagueId, CommissionerId);

        Assert.Equal(OpenDraftStatus.Success, result.Status);
        Assert.Equal(DraftStatus.Lobby, result.Snapshot!.Status);
        Assert.Equal(2, drafts.All.Count);
    }

    [Fact]
    public async Task Join_rejects_user_not_in_league()
    {
        var result = await Sut().Join(LeagueId, StrangerId);

        Assert.Equal(JoinDraftStatus.UserNotInLeague, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Join_succeeds_with_null_snapshot_when_no_draft_exists()
    {
        var result = await Sut().Join(LeagueId, MemberId);

        Assert.Equal(JoinDraftStatus.Success, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Join_returns_latest_snapshot_including_closed()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Closed });

        var result = await Sut(drafts).Join(LeagueId, MemberId);

        Assert.Equal(JoinDraftStatus.Success, result.Status);
        Assert.Equal(DraftStatus.Closed, result.Snapshot!.Status);
    }

    [Fact]
    public async Task Close_rejects_user_not_in_league()
    {
        var result = await Sut().Close(LeagueId, StrangerId);

        Assert.Equal(CloseDraftStatus.UserNotInLeague, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Close_rejects_non_commissioner()
    {
        var result = await Sut().Close(LeagueId, MemberId);

        Assert.Equal(CloseDraftStatus.NotCommissioner, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Close_rejects_when_no_draft_exists()
    {
        var result = await Sut().Close(LeagueId, CommissionerId);

        Assert.Equal(CloseDraftStatus.NoDraftFound, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Close_rejects_already_closed_draft()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Closed });

        var result = await Sut(drafts).Close(LeagueId, CommissionerId);

        Assert.Equal(CloseDraftStatus.NoDraftFound, result.Status);
        Assert.Null(result.Snapshot);
    }

    [Fact]
    public async Task Close_rejects_live_draft()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Live });

        var result = await Sut(drafts).Close(LeagueId, CommissionerId);

        Assert.Equal(CloseDraftStatus.DraftInactive, result.Status);
        Assert.Null(result.Snapshot);
        Assert.Equal(DraftStatus.Live, drafts.All.Single().Status);
    }

    [Fact]
    public async Task Close_sets_lobby_to_closed()
    {
        var drafts = new FakeDraftRepository();
        await drafts.AddAsync(new DraftEntity { LeagueId = LeagueId, Status = DraftStatus.Lobby });

        var result = await Sut(drafts).Close(LeagueId, CommissionerId);

        Assert.Equal(CloseDraftStatus.Success, result.Status);
        Assert.Equal(DraftStatus.Closed, result.Snapshot!.Status);
        Assert.Equal(DraftStatus.Closed, drafts.All.Single().Status);
    }

    private static DraftService Sut(FakeDraftRepository? drafts = null)
    {
        var leagues = new FakeLeagueRepository();
        leagues.AddMember(LeagueId, CommissionerId, "Commish", LeagueMemberRole.Commissioner);
        leagues.AddMember(LeagueId, MemberId, "Member", LeagueMemberRole.Member);
        return new DraftService(drafts ?? new FakeDraftRepository(), leagues);
    }

    private sealed class FakeDraftRepository : IDraftRepository
    {
        private int _nextId = 1;
        public List<DraftEntity> All { get; } = [];

        public Task<DraftEntity> AddAsync(DraftEntity draft, CancellationToken cancellationToken = default)
        {
            draft.Id = _nextId++;
            All.Add(draft);
            return Task.FromResult(draft);
        }

        public Task<DraftEntity> UpdateAsync(DraftEntity draft, CancellationToken cancellationToken = default) =>
            Task.FromResult(draft);

        public Task<DraftEntity?> GetAsync(int leagueId, CancellationToken cancellationToken = default) =>
            Task.FromResult(All.FirstOrDefault(draft =>
                draft.LeagueId == leagueId && draft.Status != DraftStatus.Closed));

        public Task<DraftEntity?> GetLatestAsync(int leagueId, CancellationToken cancellationToken = default) =>
            Task.FromResult(All
                .Where(draft => draft.LeagueId == leagueId)
                .OrderByDescending(draft => draft.Id)
                .FirstOrDefault());
    }

    private sealed class FakeLeagueRepository : ILeagueRepository
    {
        private readonly Dictionary<(int LeagueId, string UserId), LeagueMembership> _memberships = [];
        private readonly List<LeagueMember> _members = [];

        public void AddMember(int leagueId, string userId, string displayName, LeagueMemberRole role)
        {
            _memberships[(leagueId, userId)] = new LeagueMembership(
                new League
                {
                    JoinCode = "TESTCODE",
                    Name = "Test League",
                    Settings = new Settings { EligiblePositions = [], Rules = [] }
                },
                role);

            _members.Add(new LeagueMember
            {
                LeagueId = leagueId,
                UserId = userId,
                Role = role,
                JoinedAtUtc = DateTimeOffset.UtcNow,
                User = new User { Id = userId, DisplayName = displayName }
            });
        }

        public Task<LeagueMembership?> GetMembershipAsync(
            int leagueId,
            string userId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_memberships.GetValueOrDefault((leagueId, userId)));

        public Task<IReadOnlyList<LeagueMember>> ListMembersAsync(
            int leagueId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LeagueMember>>(
                _members.Where(member => member.LeagueId == leagueId).ToList());

        public Task<League> AddAsync(League league, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<League?> GetByIdAsync(int leagueId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<League?> GetByCodeAsync(string leagueCode, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<bool> ExistsByJoinCodeAsync(string code, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<bool> IsMemberAsync(
            int leagueId,
            string userId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<LeagueMember> AddMemberAsync(
            LeagueMember leagueMember,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<LeagueMembership>> ListForUserAsync(
            string userId,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
