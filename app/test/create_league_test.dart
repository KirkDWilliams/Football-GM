import 'package:flutter/material.dart';
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
  testWidgets('Create form asks for name and Weekly cap defaulting to 100', (
    tester,
  ) async {
    await _pumpLoggedIn(tester);

    await tester.tap(find.text('Create League'));
    await tester.pumpAndSettle();

    expect(find.widgetWithText(AppBar, 'Create League'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Name'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, 'Weekly cap'), findsOneWidget);
    expect(find.widgetWithText(TextFormField, '100'), findsOneWidget);
    expect(find.text('Scoring weights'), findsOneWidget);
    expect(find.text('Passing'), findsNothing);
    expect(find.text('Bonuses'), findsNothing);
    expect(find.text('Eligible positions'), findsNothing);
  });

  testWidgets(
    'Opened scoring-weight editor is grouped and prefilled with defaults',
    (tester) async {
      await _pumpLoggedIn(tester);

      await tester.tap(find.text('Create League'));
      await tester.pumpAndSettle();
      await tester.tap(find.text('Scoring weights'));
      await tester.pump();

      expect(find.text('Passing'), findsOneWidget);
      expect(find.text('Rushing'), findsOneWidget);
      expect(find.text('Receiving'), findsOneWidget);
      expect(find.text('Turnovers'), findsOneWidget);
      expect(find.text('Kicking'), findsOneWidget);
      expect(
        find.widgetWithText(TextFormField, 'Passing yards'),
        findsOneWidget,
      );
      expect(find.widgetWithText(TextFormField, '0.04'), findsOneWidget);
      expect(
        find.widgetWithText(TextFormField, 'Rushing yards'),
        findsOneWidget,
      );
      expect(find.widgetWithText(TextFormField, 'Receptions'), findsOneWidget);
      expect(
        find.widgetWithText(TextFormField, 'Interceptions'),
        findsOneWidget,
      );
      expect(
        find.widgetWithText(TextFormField, 'Field goals made'),
        findsOneWidget,
      );
      expect(find.widgetWithText(TextFormField, '3'), findsOneWidget);
      expect(find.text('Bonuses'), findsNothing);
      expect(find.text('Eligible positions'), findsNothing);
    },
  );

  testWidgets('Blank name stays on the Create form with an error', (
    tester,
  ) async {
    await _pumpLoggedIn(tester);

    await tester.tap(find.text('Create League'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Create'));
    await tester.pump();

    expect(find.text('Name is required'), findsOneWidget);
    expect(find.widgetWithText(AppBar, 'Create League'), findsOneWidget);
    expect(find.text('JOINCODE1'), findsNothing);
  });

  testWidgets(
    'Weekly cap of 0 or less stays on the Create form with an error',
    (tester) async {
      await _pumpLoggedIn(tester);

      await tester.tap(find.text('Create League'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Name'),
        'Sunday League',
      );
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Weekly cap'),
        '0',
      );
      await tester.tap(find.text('Create'));
      await tester.pump();

      expect(find.text('Weekly cap must be greater than 0'), findsOneWidget);
      expect(find.widgetWithText(AppBar, 'Create League'), findsOneWidget);

      await tester.enterText(
        find.widgetWithText(TextFormField, 'Weekly cap'),
        '-1',
      );
      await tester.tap(find.text('Create'));
      await tester.pump();

      expect(find.text('Weekly cap must be greater than 0'), findsOneWidget);
      expect(find.text('JOINCODE1'), findsNothing);
    },
  );

  testWidgets(
    'Successful Create opens the League screen and the list after back',
    (tester) async {
      await _pumpLoggedIn(tester);

      await tester.tap(find.text('Create League'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Name'),
        'Sunday League',
      );
      await tester.tap(find.text('Create'));
      await tester.pumpAndSettle();

      expect(find.text('JOINCODE1'), findsOneWidget);
      expect(find.text('Commissioner'), findsWidgets);
      expect(find.text('Weekly cap'), findsOneWidget);
      expect(find.text('100'), findsWidgets);
      expect(find.text('Quarterback'), findsOneWidget);
      expect(find.text('Scoring weights'), findsOneWidget);
      expect(find.text('Save'), findsNothing);

      await tester.pageBack();
      await tester.pumpAndSettle();

      expect(find.text('My Leagues'), findsOneWidget);
      expect(find.text('Sunday League'), findsOneWidget);
      expect(find.text('JOINCODE1'), findsOneWidget);
      expect(find.text('Commissioner'), findsOneWidget);
      expect(find.text('Standard scoring'), findsOneWidget);
    },
  );

  testWidgets(
    'Changing a scoring weight creates a League with Custom scoring',
    (tester) async {
      await _pumpLoggedIn(tester);

      await tester.tap(find.text('Create League'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Name'),
        'Sunday League',
      );
      await tester.tap(find.text('Scoring weights'));
      await tester.pump();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Passing yards'),
        '0.05',
      );
      await tester.ensureVisible(find.widgetWithText(FilledButton, 'Create'));
      await tester.tap(find.widgetWithText(FilledButton, 'Create'));
      await tester.pumpAndSettle();

      await tester.pageBack();
      await tester.pumpAndSettle();

      expect(find.text('Sunday League'), findsOneWidget);
      expect(find.text('Custom scoring'), findsOneWidget);
      expect(find.text('Standard scoring'), findsNothing);
    },
  );
}

Future<void> _pumpLoggedIn(WidgetTester tester) async {
  final auth = _auth();
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: _FakeLeagueApi(),
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
  final List<LeagueSummary> _leagues = [];
  final Map<int, LeagueDetails> _details = {};
  var _nextId = 1;

  @override
  Future<List<LeagueSummary>> listMyLeagues() async => _leagues;

  @override
  Future<LeagueDetails> getLeague(int leagueId) async {
    final league = _details[leagueId];
    if (league == null) {
      throw Exception('League $leagueId not found');
    }
    return league;
  }

  @override
  Future<int> createLeague({
    required String name,
    required num weeklyCap,
    List<ScoringWeightRule>? scoringWeights,
  }) async {
    final id = _nextId++;
    const joinCode = 'JOINCODE1';
    final rules = scoringWeights ?? ScoringWeightRule.defaults();
    _details[id] = LeagueDetails(
      leagueId: id,
      name: name,
      joinCode: joinCode,
      weeklyCap: weeklyCap,
      role: LeagueRole.commissioner,
      positions: const [
        Position.quarterback,
        Position.runningBack,
        Position.wideReceiver,
        Position.tightEnd,
        Position.kicker,
      ],
      rules: rules,
    );
    _leagues.add(
      LeagueSummary(
        leagueId: id,
        name: name,
        joinCode: joinCode,
        role: LeagueRole.commissioner,
        scoring: ScoringWeightRule.usesDefaultScoringWeights(rules)
            ? ScoringKind.standard
            : ScoringKind.custom,
      ),
    );
    return id;
  }

  @override
  Future<int> joinLeague(String joinCode) async {
    throw UnsupportedError('joinLeague is not used by Create tests');
  }
}
