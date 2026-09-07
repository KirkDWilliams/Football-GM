import 'package:flutter/services.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/models/auth_user.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/core/network/api_client.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

void main() {
  testWidgets('Tapping a league opens its information screen from the id get', (
    tester,
  ) async {
    await _pumpLoggedIn(
      tester,
      leagues: const [
        LeagueSummary(
          leagueId: 1,
          name: 'Sunday League',
          joinCode: 'LISTCODE',
          role: LeagueRole.commissioner,
          scoring: ScoringKind.standard,
        ),
      ],
      details: {
        1: const LeagueDetails(
          leagueId: 1,
          name: 'Sunday League',
          joinCode: 'JOINCODE1',
          weeklyCap: 125,
          role: LeagueRole.commissioner,
          positions: [
            Position.quarterback,
            Position.runningBack,
            Position.runningBack,
            Position.wideReceiver,
          ],
          rules: [ScoringWeightRule(stat: StatType.passingYards, weight: 0.04)],
        ),
      },
    );

    await tester.tap(find.text('Sunday League'));
    await tester.pumpAndSettle();

    expect(find.text('Name'), findsOneWidget);
    expect(find.text('JOINCODE1'), findsOneWidget);
    expect(find.text('Commissioner'), findsWidgets);
    expect(find.text('Weekly cap'), findsOneWidget);
    expect(find.text('125'), findsOneWidget);
    expect(find.text('Eligible positions'), findsOneWidget);
    expect(find.text('Quarterback'), findsOneWidget);
    expect(find.text('Running Back'), findsNWidgets(2));
    expect(find.text('Wide Receiver'), findsOneWidget);
    expect(find.text('Scoring weights'), findsOneWidget);
    expect(find.text('Save'), findsNothing);
    expect(find.text('Edit'), findsNothing);
  });

  testWidgets('Join code on the league screen is copyable for a Member', (
    tester,
  ) async {
    String? clipboard;
    tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
      SystemChannels.platform,
      (call) async {
        if (call.method == 'Clipboard.setData') {
          clipboard = (call.arguments as Map)['text'] as String?;
        }
        return null;
      },
    );
    addTearDown(() {
      tester.binding.defaultBinaryMessenger.setMockMethodCallHandler(
        SystemChannels.platform,
        null,
      );
    });

    await _pumpLoggedIn(
      tester,
      leagues: const [
        LeagueSummary(
          leagueId: 2,
          name: 'Joined League',
          joinCode: 'LISTCODE',
          role: LeagueRole.member,
          scoring: ScoringKind.standard,
        ),
      ],
      details: {
        2: const LeagueDetails(
          leagueId: 2,
          name: 'Joined League',
          joinCode: 'MEMBERCD',
          weeklyCap: 100,
          role: LeagueRole.member,
          positions: [Position.kicker],
          rules: [],
        ),
      },
    );

    await tester.tap(find.text('Joined League'));
    await tester.pumpAndSettle();

    expect(find.text('MEMBERCD'), findsOneWidget);
    expect(find.text('Member'), findsWidgets);
    expect(find.text('Scoring weights'), findsOneWidget);

    await tester.tap(find.text('MEMBERCD'));
    await tester.pump();

    expect(clipboard, 'MEMBERCD');
  });

  testWidgets('Back from a league returns to the leagues list', (tester) async {
    await _pumpLoggedIn(
      tester,
      leagues: const [
        LeagueSummary(
          leagueId: 1,
          name: 'Sunday League',
          joinCode: 'ABCD1234',
          role: LeagueRole.commissioner,
          scoring: ScoringKind.standard,
        ),
      ],
      details: {
        1: const LeagueDetails(
          leagueId: 1,
          name: 'Sunday League',
          joinCode: 'ABCD1234',
          weeklyCap: 100,
          role: LeagueRole.commissioner,
          positions: [Position.quarterback],
          rules: [],
        ),
      },
    );

    await tester.tap(find.text('Sunday League'));
    await tester.pumpAndSettle();
    expect(find.text('Weekly cap'), findsOneWidget);

    await tester.pageBack();
    await tester.pumpAndSettle();

    expect(find.text('My Leagues'), findsOneWidget);
    expect(find.text('Sunday League'), findsOneWidget);
    expect(find.text('Weekly cap'), findsNothing);
  });

  testWidgets(
    'Scoring weights stay collapsed until opened and group by category',
    (tester) async {
      await _pumpLoggedIn(
        tester,
        leagues: const [
          LeagueSummary(
            leagueId: 1,
            name: 'Sunday League',
            joinCode: 'ABCD1234',
            role: LeagueRole.commissioner,
            scoring: ScoringKind.standard,
          ),
        ],
        details: {
          1: const LeagueDetails(
            leagueId: 1,
            name: 'Sunday League',
            joinCode: 'ABCD1234',
            weeklyCap: 100,
            role: LeagueRole.commissioner,
            positions: [Position.quarterback],
            rules: [
              ScoringWeightRule(stat: StatType.passingYards, weight: 0.04),
              ScoringWeightRule(stat: StatType.rushingYards, weight: 0.1),
              ScoringWeightRule(stat: StatType.receptions, weight: 0),
              ScoringWeightRule(stat: StatType.interceptions, weight: -2),
              ScoringWeightRule(stat: StatType.fieldGoalsMade, weight: 3),
            ],
          ),
        },
      );

      await tester.tap(find.text('Sunday League'));
      await tester.pumpAndSettle();

      expect(find.text('Scoring weights'), findsOneWidget);
      expect(find.text('Passing'), findsNothing);
      expect(find.text('Passing yards'), findsNothing);
      expect(find.text('Bonuses'), findsNothing);

      await tester.tap(find.text('Scoring weights'));
      await tester.pump();

      expect(find.text('Passing'), findsOneWidget);
      expect(find.text('Rushing'), findsOneWidget);
      expect(find.text('Receiving'), findsOneWidget);
      expect(find.text('Turnovers'), findsOneWidget);
      expect(find.text('Kicking'), findsOneWidget);
      expect(find.text('Passing yards'), findsOneWidget);
      expect(find.text('0.04'), findsOneWidget);
    },
  );

  testWidgets('Bonuses accordion appears only when bonus rules exist', (
    tester,
  ) async {
    await _pumpLoggedIn(
      tester,
      leagues: const [
        LeagueSummary(
          leagueId: 1,
          name: 'Plain League',
          joinCode: 'PLAIN001',
          role: LeagueRole.commissioner,
          scoring: ScoringKind.standard,
        ),
        LeagueSummary(
          leagueId: 2,
          name: 'Bonus League',
          joinCode: 'BONUS001',
          role: LeagueRole.member,
          scoring: ScoringKind.standard,
        ),
      ],
      details: {
        1: const LeagueDetails(
          leagueId: 1,
          name: 'Plain League',
          joinCode: 'PLAIN001',
          weeklyCap: 100,
          role: LeagueRole.commissioner,
          positions: [Position.quarterback],
          rules: [ScoringWeightRule(stat: StatType.passingYards, weight: 0.04)],
        ),
        2: const LeagueDetails(
          leagueId: 2,
          name: 'Bonus League',
          joinCode: 'BONUS001',
          weeklyCap: 100,
          role: LeagueRole.member,
          positions: [Position.quarterback],
          rules: [
            ScoringWeightRule(stat: StatType.passingYards, weight: 0.04),
            BonusRule(stat: StatType.passingYards, threshold: 300, points: 3),
          ],
        ),
      },
    );

    await tester.tap(find.text('Plain League'));
    await tester.pumpAndSettle();
    expect(find.text('Bonuses'), findsNothing);

    await tester.pageBack();
    await tester.pumpAndSettle();

    await tester.tap(find.text('Bonus League'));
    await tester.pumpAndSettle();
    expect(find.text('Bonuses'), findsOneWidget);
    expect(find.text('Passing yards'), findsNothing);

    await tester.tap(find.text('Bonuses'));
    await tester.pump();
    expect(find.text('Passing yards'), findsOneWidget);
    expect(find.text('3 points at 300'), findsOneWidget);
  });
}

Future<void> _pumpLoggedIn(
  WidgetTester tester, {
  required List<LeagueSummary> leagues,
  required Map<int, LeagueDetails> details,
}) async {
  final auth = _auth();
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: _FakeLeagueApi(leagues: leagues, details: details),
    ),
  );
  await tester.pumpAndSettle();
}

({AuthController controller, AuthService service}) _auth() {
  final tokenStore = TokenStore();
  final apiClient = ApiClient(
    baseUrl: ApiConfig.baseUrl,
    tokenStore: tokenStore,
  );
  final service = AuthService(apiClient: apiClient, tokenStore: tokenStore);
  final controller = AuthController(authService: service)
    ..status = AuthStatus.authenticated
    ..user = const AuthUser(
      id: 'user-1',
      email: 'gm@example.com',
      displayName: 'Nick',
    );
  return (controller: controller, service: service);
}

class _FakeLeagueApi implements LeagueApi {
  _FakeLeagueApi({required this.leagues, required this.details});

  final List<LeagueSummary> leagues;
  final Map<int, LeagueDetails> details;

  @override
  Future<List<LeagueSummary>> listMyLeagues() async => leagues;

  @override
  Future<LeagueDetails> getLeague(int leagueId) async {
    final league = details[leagueId];
    if (league == null) {
      throw Exception('League $leagueId not found');
    }
    return league;
  }
}
