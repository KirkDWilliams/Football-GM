import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/features/draft/draft_api.dart';
import 'package:football_gm_app/features/leagues/league_api.dart';
import 'package:football_gm_app/models/bid.dart';
import 'package:football_gm_app/models/player.dart';

import 'logged_in_auth.dart';

Future<({AuthController controller, AuthService service})> pumpApp(
  WidgetTester tester, {
  required LeagueApi leagueApi,
  DraftApi? draftApi,
  AuthStatus status = AuthStatus.authenticated,
  bool openLeagues = false,
}) async {
  final auth = loggedInAuth(status: status);
  await tester.pumpWidget(
    FootballGmApp(
      authController: auth.controller,
      authService: auth.service,
      leagueApi: leagueApi,
      draftApi: draftApi ?? _NoOpDraftApi(),
    ),
  );
  await tester.pumpAndSettle();
  if (openLeagues) {
    await tester.tap(find.text('Leagues'));
    await tester.pumpAndSettle();
  }
  return auth;
}

class _NoOpDraftApi implements DraftApi {
  @override
  Future<List<Player>> getAvailablePlayers(int leagueId) async => [];

  @override
  Future<void> startAuction(int leagueId, String playerId) async {}

  @override
  Future<List<Bid>> getAuctionBids(int leagueId, String playerId) async => [];

  @override
  Future<void> placeBid(
    int leagueId,
    int userId,
    String playerId,
    num duration,
    num salary,
    num? signingBonus,
  ) async {}

  @override
  Future<void> passAuction(int leagueId, int userId) async {}

  @override
  Future<void> processAuction(
    int leagueId,
    int userId,
    List<Bid> winningBids,
  ) async {}
}
