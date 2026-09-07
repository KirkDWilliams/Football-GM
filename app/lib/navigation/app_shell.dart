import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/screens/login_screen.dart';
import 'package:football_gm_app/features/home/home_page.dart';
import 'package:football_gm_app/leagues/screens/leagues_page.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:provider/provider.dart';

/// Root route: empty home, leagues, or login based on the nav — never a gate.
class AppShell extends StatelessWidget {
  const AppShell({super.key});

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthController>();
    final nav = context.watch<NavigationController>();

    if (auth.status == AuthStatus.unknown) {
      return const ArcadePage(
        centerBody: true,
        body: Center(child: CircularProgressIndicator()),
      );
    }

    return switch (nav.section) {
      AppSection.home => const HomePage(),
      AppSection.leagues => const LeaguesPage(),
      AppSection.login =>
        auth.status == AuthStatus.authenticated
            ? const HomePage()
            : const LoginScreen(),
    };
  }
}
