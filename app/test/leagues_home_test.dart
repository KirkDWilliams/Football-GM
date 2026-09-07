import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/models/auth_user.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

import 'logged_in_auth.dart';

void main() {
  testWidgets(
    'Signed-in user with no leagues sees an empty list with Create and Join',
    (tester) async {
      await _pumpLoggedIn(tester, leagues: const []);

      expect(find.text('My Leagues'), findsOneWidget);
      expect(find.text('Create League'), findsOneWidget);
      expect(find.text('Join with code'), findsOneWidget);
      expect(find.text('Could not load Leagues'), findsNothing);
      expect(find.text('Teams'), findsNothing);
      expect(find.text('Sync'), findsNothing);
      expect(find.text('No teams found'), findsNothing);
    },
  );

  testWidgets('Failed list get is not shown as belonging to no Leagues', (
    tester,
  ) async {
    await _pumpLoggedIn(tester, listError: Exception('network'));

    expect(find.text('Could not load Leagues'), findsOneWidget);
    expect(find.text('Create League'), findsOneWidget);
    expect(find.text('Join with code'), findsOneWidget);
  });

  testWidgets(
    'Signed-in user sees each league name, join code, role, and scoring',
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
          LeagueSummary(
            leagueId: 2,
            name: 'Custom League',
            joinCode: 'WXYZ9876',
            role: LeagueRole.member,
            scoring: ScoringKind.custom,
          ),
        ],
      );

      expect(find.text('Sunday League'), findsOneWidget);
      expect(find.text('ABCD1234'), findsOneWidget);
      expect(find.text('Commissioner'), findsOneWidget);
      expect(find.text('Standard scoring'), findsOneWidget);
      expect(find.text('Custom League'), findsOneWidget);
      expect(find.text('WXYZ9876'), findsOneWidget);
      expect(find.text('Member'), findsOneWidget);
      expect(find.text('Custom scoring'), findsOneWidget);
      expect(find.text('Create League'), findsOneWidget);
      expect(find.text('Join with code'), findsOneWidget);
      expect(find.text('PassingYards'), findsNothing);
    },
  );

  testWidgets('Create and Join open as two separate actions', (tester) async {
    await _pumpLoggedIn(tester, leagues: const []);

    await tester.tap(find.text('Create League'));
    await tester.pumpAndSettle();
    expect(find.text('Name'), findsOneWidget);
    expect(find.text('Weekly cap'), findsOneWidget);
    expect(find.text('Join code'), findsNothing);

    await tester.pageBack();
    await tester.pumpAndSettle();

    await tester.tap(find.text('Join with code'));
    await tester.pumpAndSettle();
    expect(find.text('Join code'), findsOneWidget);
    expect(find.text('Weekly cap'), findsNothing);
    expect(find.text('Name'), findsNothing);
  });

  testWidgets('Failed list reload keeps the Leagues already on screen', (
    tester,
  ) async {
    final api = _FakeLeagueApi([
      const LeagueSummary(
        leagueId: 1,
        name: 'Sunday League',
        joinCode: 'ABCD1234',
        role: LeagueRole.commissioner,
        scoring: ScoringKind.standard,
      ),
    ]);
    final auth = loggedInAuth();

    await tester.pumpWidget(
      FootballGmApp(
        authController: auth.controller,
        authService: auth.service,
        leagueApi: api,
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Leagues'));
    await tester.pumpAndSettle();
    expect(find.text('Sunday League'), findsOneWidget);

    api.listError = Exception('network');
    await tester.tap(find.text('Create League'));
    await tester.pumpAndSettle();
    await tester.pageBack();
    await tester.pumpAndSettle();

    expect(find.text('Sunday League'), findsOneWidget);
    expect(find.text('Could not load Leagues'), findsOneWidget);
  });

  testWidgets('Signing in loads leagues from the list get', (tester) async {
    final api = _FakeLeagueApi([
      const LeagueSummary(
        leagueId: 1,
        name: 'Sunday League',
        joinCode: 'ABCD1234',
        role: LeagueRole.commissioner,
        scoring: ScoringKind.standard,
      ),
    ]);
    final auth = loggedInAuth();

    await tester.pumpWidget(
      FootballGmApp(
        authController: auth.controller,
        authService: auth.service,
        leagueApi: api,
      ),
    );
    await tester.pumpAndSettle();
    await tester.tap(find.text('Leagues'));
    await tester.pumpAndSettle();
    expect(find.text('Sunday League'), findsOneWidget);

    auth.controller
      ..user = null
      ..status = AuthStatus.unauthenticated
      ..notifyListeners();
    await tester.pumpAndSettle();
    expect(find.text('Sign in'), findsWidgets);

    api.leagues = [
      const LeagueSummary(
        leagueId: 2,
        name: 'Custom League',
        joinCode: 'WXYZ9876',
        role: LeagueRole.member,
        scoring: ScoringKind.custom,
      ),
    ];
    auth.controller
      ..user = const AuthUser(
        id: 'user-2',
        email: 'other@example.com',
        displayName: 'Sam',
      )
      ..status = AuthStatus.authenticated
      ..notifyListeners();
    await tester.pumpAndSettle();

    expect(find.text('Custom League'), findsOneWidget);
    expect(find.text('WXYZ9876'), findsOneWidget);
    expect(find.text('Sunday League'), findsNothing);
  });

  testWidgets('Signed-in user still has change password and sign out', (
    tester,
  ) async {
    await _pumpLoggedIn(tester, leagues: const []);

    await tester.tap(find.byTooltip('Account'));
    await tester.pumpAndSettle();
    expect(find.text('Change password'), findsOneWidget);
    expect(find.text('Sign out'), findsOneWidget);

    await tester.tap(find.text('Change password'));
    await tester.pumpAndSettle();
    expect(find.text('Update password'), findsOneWidget);
  });
}

Future<void> _pumpLoggedIn(
  WidgetTester tester, {
  List<LeagueSummary> leagues = const [],
  Object? listError,
}) async {
  final auth = loggedInAuth();
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: _FakeLeagueApi(leagues, listError: listError),
    ),
  );
  await tester.pumpAndSettle();
  await tester.tap(find.text('Leagues'));
  await tester.pumpAndSettle();
}

class _FakeLeagueApi implements LeagueApi {
  _FakeLeagueApi(this.leagues, {this.listError});

  List<LeagueSummary> leagues;
  Object? listError;

  @override
  Future<List<LeagueSummary>> listMyLeagues() async {
    final error = listError;
    if (error != null) throw error;
    return leagues;
  }

  @override
  Future<LeagueDetails> getLeague(int leagueId) async {
    throw UnsupportedError('getLeague is not used by home tests');
  }

  @override
  Future<int> createLeague({
    required String name,
    required num weeklyCap,
    List<ScoringWeightRule>? scoringWeights,
  }) async {
    throw UnsupportedError('createLeague is not used by home tests');
  }

  @override
  Future<int> joinLeague(String joinCode) async {
    throw UnsupportedError('joinLeague is not used by home tests');
  }
}
