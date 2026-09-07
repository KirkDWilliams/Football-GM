import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

import 'logged_in_auth.dart';

void main() {
  testWidgets('Join is its own screen with a Join code field', (tester) async {
    await _pumpLoggedIn(tester);

    await tester.tap(find.text('Join with code'));
    await tester.pumpAndSettle();

    expect(find.text('Join with code'), findsOneWidget);
    expect(find.text('Join code'), findsOneWidget);
    expect(find.text('Name'), findsNothing);
    expect(find.text('Weekly cap'), findsNothing);
  });

  testWidgets('Empty Join code stays on the Join form with an error', (
    tester,
  ) async {
    await _pumpLoggedIn(tester);

    await tester.tap(find.text('Join with code'));
    await tester.pumpAndSettle();
    await tester.tap(find.text('Join'));
    await tester.pump();

    expect(find.text('Join code is required'), findsOneWidget);
    expect(find.text('Join with code'), findsOneWidget);
    expect(find.text('JOINCODE1'), findsNothing);
  });

  testWidgets('Unknown Join code stays on the Join form with an error', (
    tester,
  ) async {
    await _pumpLoggedIn(tester);

    await tester.tap(find.text('Join with code'));
    await tester.pumpAndSettle();
    await tester.enterText(
      find.widgetWithText(TextFormField, 'Join code'),
      'NOPECODE',
    );
    await tester.tap(find.text('Join'));
    await tester.pumpAndSettle();

    expect(find.text('Unknown Join code'), findsOneWidget);
    expect(find.text('Join with code'), findsOneWidget);
    expect(find.text('JOINCODE1'), findsNothing);
  });

  testWidgets(
    'Successful Join opens the League screen as a Member and the list after back',
    (tester) async {
      await _pumpLoggedIn(tester);

      await tester.tap(find.text('Join with code'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Join code'),
        'JOINCODE1',
      );
      await tester.tap(find.text('Join'));
      await tester.pumpAndSettle();

      expect(find.text('JOINCODE1'), findsOneWidget);
      expect(find.text('Member'), findsWidgets);
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
      expect(find.text('Member'), findsOneWidget);
      expect(find.text('Standard scoring'), findsOneWidget);
    },
  );

  testWidgets(
    'Already a Member opens that League instead of staying on an error',
    (tester) async {
      await _pumpLoggedIn(tester, alreadyMember: true);

      await tester.tap(find.text('Join with code'));
      await tester.pumpAndSettle();
      await tester.enterText(
        find.widgetWithText(TextFormField, 'Join code'),
        'JOINCODE1',
      );
      await tester.tap(find.text('Join'));
      await tester.pumpAndSettle();

      expect(find.text('Unknown Join code'), findsNothing);
      expect(find.text('Could not join League'), findsNothing);
      expect(find.text('Join with code'), findsNothing);
      expect(find.text('JOINCODE1'), findsOneWidget);
      expect(find.text('Member'), findsWidgets);
      expect(find.text('Weekly cap'), findsOneWidget);
      expect(find.text('Save'), findsNothing);
    },
  );
}

Future<void> _pumpLoggedIn(
  WidgetTester tester, {
  bool alreadyMember = false,
}) async {
  final auth = loggedInAuth();
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: _FakeLeagueApi(alreadyMember: alreadyMember),
    ),
  );
  await tester.pumpAndSettle();
  await tester.tap(find.text('Leagues'));
  await tester.pumpAndSettle();
}

class _FakeLeagueApi implements LeagueApi {
  _FakeLeagueApi({bool alreadyMember = false}) {
    if (alreadyMember) {
      _addMembership();
    }
  }

  final List<LeagueSummary> _leagues = [];
  final Map<int, LeagueDetails> _details = {};

  static const _joinable = LeagueDetails(
    leagueId: 1,
    name: 'Sunday League',
    joinCode: 'JOINCODE1',
    weeklyCap: 100,
    role: LeagueRole.member,
    positions: [
      Position.quarterback,
      Position.runningBack,
      Position.wideReceiver,
      Position.tightEnd,
      Position.kicker,
    ],
    rules: [ScoringWeightRule(stat: StatType.passingYards, weight: 0.04)],
  );

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
    throw UnsupportedError('createLeague is not used by Join tests');
  }

  @override
  Future<int> joinLeague(String joinCode) async {
    if (joinCode != _joinable.joinCode) {
      throw UnknownJoinCodeException();
    }
    _addMembership();
    return _joinable.leagueId;
  }

  void _addMembership() {
    _details[_joinable.leagueId] = _joinable;
    if (_leagues.any((league) => league.leagueId == _joinable.leagueId)) {
      return;
    }
    _leagues.add(
      LeagueSummary(
        leagueId: _joinable.leagueId,
        name: _joinable.name,
        joinCode: _joinable.joinCode,
        role: LeagueRole.member,
        scoring: ScoringKind.standard,
      ),
    );
  }
}
