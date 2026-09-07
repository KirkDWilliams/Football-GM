import 'package:flutter/material.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/core/network/api_client.dart';
import 'package:football_gm_app/leagues/league_api.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

  final tokenStore = TokenStore();
  await tokenStore.load();

  final apiClient = ApiClient(
    baseUrl: ApiConfig.baseUrl,
    tokenStore: tokenStore,
  );
  final authService = AuthService(
    apiClient: apiClient,
    tokenStore: tokenStore,
  );
  final authController = AuthController(authService: authService);
  await authController.bootstrap();

  runApp(
    FootballGmApp(
      authController: authController,
      authService: authService,
      leagueApi: HttpLeagueApi(dio: apiClient.dio),
    ),
  );
}
