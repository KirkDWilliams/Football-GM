import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/screens/auth_gate.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/leagues_provider.dart';
import 'package:provider/provider.dart';

/// Root widget: providers + [AuthGate].
class FootballGmApp extends StatelessWidget {
  const FootballGmApp({
    super.key,
    required this.authController,
    required this.authService,
    required this.leagueApi,
  });

  final AuthController authController;
  final AuthService authService;
  final LeagueApi leagueApi;

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider<AuthController>.value(value: authController),
        Provider<AuthService>.value(value: authService),
        Provider<TokenStore>.value(value: authService.tokenStore),
        ChangeNotifierProvider<LeaguesProvider>(
          create: (_) => LeaguesProvider(leagueApi),
        ),
      ],
      child: MaterialApp(
        title: 'Football GM',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
          useMaterial3: true,
        ),
        home: const AuthGate(),
      ),
    );
  }
}
