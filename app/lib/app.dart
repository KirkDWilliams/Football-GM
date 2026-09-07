import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/screens/change_password_screen.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/leagues_provider.dart';
import 'package:football_gm_app/navigation/app_routes.dart';
import 'package:football_gm_app/navigation/app_shell.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/arcade_theme.dart';
import 'package:provider/provider.dart';

/// Root widget: providers + themed [MaterialApp] + [AppShell].
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
        Provider<LeagueApi>.value(value: leagueApi),
        ChangeNotifierProvider<LeaguesProvider>(
          create: (_) => LeaguesProvider(leagueApi),
        ),
        ChangeNotifierProvider<NavigationController>(
          create: (_) => NavigationController(),
        ),
      ],
      child: MaterialApp(
        title: 'Football GM',
        debugShowCheckedModeBanner: false,
        theme: ArcadeTheme.dark(),
        home: const AppShell(),
        routes: {AppRoutes.changePassword: (_) => const ChangePasswordScreen()},
      ),
    );
  }
}
