import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

import 'logged_in_auth.dart';

void main() {
  testWidgets('Signed-out users see the login screen', (tester) async {
    final auth = loggedInAuth(status: AuthStatus.unauthenticated);

    await tester.pumpWidget(
      FootballGmApp(
        authController: auth.controller,
        authService: auth.service,
        leagueApi: _EmptyLeagueApi(),
      ),
    );
    await tester.pump();

    expect(find.text('Sign in'), findsWidgets);
    expect(find.text('Create an account'), findsOneWidget);
    expect(find.text('Sync'), findsNothing);
    expect(find.text('My Leagues'), findsNothing);
  });
}

class _EmptyLeagueApi implements LeagueApi {
  @override
  Future<List<LeagueSummary>> listMyLeagues() async => const [];

  @override
  Future<LeagueDetails> getLeague(int leagueId) async {
    throw UnsupportedError('getLeague is not used by signed-out tests');
  }

  @override
  Future<int> createLeague({
    required String name,
    required num weeklyCap,
    List<ScoringWeightRule>? scoringWeights,
  }) async {
    throw UnsupportedError('createLeague is not used by signed-out tests');
  }

  @override
  Future<int> joinLeague(String joinCode) async {
    throw UnsupportedError('joinLeague is not used by signed-out tests');
  }
}
