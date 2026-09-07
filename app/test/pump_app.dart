import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/features/leagues/league_api.dart';

import 'logged_in_auth.dart';

Future<({AuthController controller, AuthService service})> pumpApp(
  WidgetTester tester, {
  required LeagueApi leagueApi,
  AuthStatus status = AuthStatus.authenticated,
  bool openLeagues = false,
}) async {
  final auth = loggedInAuth(status: status);
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: leagueApi,
    ),
  );
  await tester.pumpAndSettle();
  if (openLeagues) {
    await tester.tap(find.text('Leagues'));
    await tester.pumpAndSettle();
  }
  return auth;
}
