import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/navigation/app_routes.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/arcade_assets.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';
import 'package:football_gm_app/ui/arcade_fonts.dart';
import 'package:provider/provider.dart';

/// Persistent website nav: Home, Leagues, and Login / account.
class GameNavBar extends StatelessWidget {
  const GameNavBar({super.key});

  @override
  Widget build(BuildContext context) {
    final nav = context.watch<NavigationController>();
    final auth = context.watch<AuthController>();
    final signedIn = auth.status == AuthStatus.authenticated;
    final narrow = MediaQuery.sizeOf(context).width < 720;

    return Material(
      color: ArcadeColors.turf,
      child: Container(
        decoration: const BoxDecoration(
          border: Border(
            top: BorderSide(color: ArcadeColors.bevelLight, width: 2),
            bottom: BorderSide(color: ArcadeColors.gold, width: 3),
          ),
        ),
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
        child: Row(
          children: [
            _Brand(onTap: () => _go(context, AppSection.home)),
            const Spacer(),
            if (narrow) ...[
              PopupMenuButton<AppSection>(
                tooltip: 'Menu',
                icon: const Icon(Icons.menu, color: ArcadeColors.gold),
                onSelected: (section) => _go(context, section),
                itemBuilder: (_) => [
                  const PopupMenuItem(
                    value: AppSection.home,
                    child: Text('Home'),
                  ),
                  const PopupMenuItem(
                    value: AppSection.leagues,
                    child: Text('Leagues'),
                  ),
                  if (!signedIn)
                    const PopupMenuItem(
                      value: AppSection.login,
                      child: Text('Login'),
                    ),
                ],
              ),
              if (signedIn) _AccountMenu(auth: auth),
            ] else ...[
              _NavLink(
                label: 'Home',
                selected: nav.section == AppSection.home,
                onTap: () => _go(context, AppSection.home),
              ),
              _NavLink(
                label: 'Leagues',
                selected: nav.section == AppSection.leagues,
                onTap: () => _go(context, AppSection.leagues),
              ),
              if (signedIn)
                _AccountMenu(auth: auth)
              else
                _NavLink(
                  label: 'Login',
                  selected: nav.section == AppSection.login,
                  onTap: () => _go(context, AppSection.login),
                ),
            ],
          ],
        ),
      ),
    );
  }

  void _go(BuildContext context, AppSection section) {
    context.read<NavigationController>().go(section, context);
  }
}

class _Brand extends StatelessWidget {
  const _Brand({required this.onTap});

  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      onTap: onTap,
      child: Row(
        children: [
          Image.asset(
            ArcadeAssets.football,
            height: 28,
            filterQuality: FilterQuality.none,
            errorBuilder: (_, _, _) => const SizedBox(width: 36, height: 36),
          ),
          const SizedBox(width: 10),
          Text(
            'FOOTBALL GM',
            style: ArcadeFonts.title(size: 12, color: ArcadeColors.gold),
          ),
        ],
      ),
    );
  }
}

class _NavLink extends StatelessWidget {
  const _NavLink({
    required this.label,
    required this.selected,
    required this.onTap,
  });

  final String label;
  final bool selected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final color = selected ? ArcadeColors.gold : ArcadeColors.cream;

    return Padding(
      padding: const EdgeInsets.only(left: 8),
      child: TextButton(
        onPressed: onTap,
        style: TextButton.styleFrom(
          foregroundColor: color,
          padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(label, style: ArcadeFonts.title(size: 10, color: color)),
            const SizedBox(height: 4),
            AnimatedContainer(
              duration: const Duration(milliseconds: 150),
              height: 3,
              width: selected ? 28 : 0,
              color: ArcadeColors.gold,
            ),
          ],
        ),
      ),
    );
  }
}

class _AccountMenu extends StatelessWidget {
  const _AccountMenu({required this.auth});

  final AuthController auth;

  @override
  Widget build(BuildContext context) {
    final name = auth.user?.displayName ?? 'GM';

    return Padding(
      padding: const EdgeInsets.only(left: 8),
      child: PopupMenuButton<_AccountAction>(
        tooltip: 'Account',
        enabled: !auth.busy,
        offset: const Offset(0, 44),
        onSelected: (item) async {
          switch (item) {
            case _AccountAction.changePassword:
              auth.clearError();
              await Navigator.of(context).pushNamed(AppRoutes.changePassword);
            case _AccountAction.signOut:
              await auth.logout();
          }
        },
        itemBuilder: (_) => const [
          PopupMenuItem(
            value: _AccountAction.changePassword,
            child: Text('Change password'),
          ),
          PopupMenuItem(value: _AccountAction.signOut, child: Text('Sign out')),
        ],
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 8),
          child: Row(
            children: [
              Image.asset(
                ArcadeAssets.helmet,
                height: 28,
                filterQuality: FilterQuality.none,
                errorBuilder: (_, _, _) => const Icon(Icons.sports_football),
              ),
              const SizedBox(width: 8),
              Text(
                name,
                style: ArcadeFonts.title(size: 10, color: ArcadeColors.gold),
              ),
              const Icon(Icons.arrow_drop_down, color: ArcadeColors.gold),
            ],
          ),
        ),
      ),
    );
  }
}

enum _AccountAction { changePassword, signOut }
