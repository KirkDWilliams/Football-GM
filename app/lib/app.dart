import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/screens/auth_gate.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/data/team_repository.dart';
import 'package:football_gm_app/features/home/teams_provider.dart';
import 'package:provider/provider.dart';

/// Root widget: providers + [AuthGate].
class FootballGmApp extends StatelessWidget {
  const FootballGmApp({
    super.key,
    required this.authController,
    required this.authService,
    required this.teamRepository,
  });

  final AuthController authController;
  final AuthService authService;
  final TeamRepository teamRepository;

  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider<AuthController>.value(value: authController),
        Provider<AuthService>.value(value: authService),
        Provider<TokenStore>.value(value: authService.tokenStore),
        Provider<TeamRepository>.value(value: teamRepository),
        ChangeNotifierProvider<TeamsProvider>(
          create: (_) => TeamsProvider(teamRepository),
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
