import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/leagues_provider.dart';
import 'package:football_gm_app/leagues/screens/create_league_screen.dart';
import 'package:football_gm_app/leagues/screens/join_league_screen.dart';
import 'package:football_gm_app/leagues/screens/league_information_screen.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/arcade_assets.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';
import 'package:football_gm_app/ui/widgets/status_banner.dart';
import 'package:provider/provider.dart';

class LeaguesPage extends StatefulWidget {
  const LeaguesPage({super.key});

  @override
  State<LeaguesPage> createState() => _LeaguesPageState();
}

class _LeaguesPageState extends State<LeaguesPage> {
  late final AuthController _auth;

  @override
  void initState() {
    super.initState();
    _auth = context.read<AuthController>()..addListener(_onAuth);
    WidgetsBinding.instance.addPostFrameCallback((_) => _loadIfSignedIn());
  }

  @override
  void dispose() {
    _auth.removeListener(_onAuth);
    super.dispose();
  }

  void _onAuth() {
    if (!mounted) return;
    if (_auth.status == AuthStatus.authenticated) {
      context.read<LeaguesProvider>().reload();
    }
    setState(() {});
  }

  void _loadIfSignedIn() {
    if (!mounted) return;
    if (_auth.status == AuthStatus.authenticated) {
      context.read<LeaguesProvider>().reload();
    }
  }

  Future<void> _openAndReload(Widget screen) async {
    await Navigator.of(
      context,
    ).push(MaterialPageRoute<void>(builder: (_) => screen));
    if (!mounted) return;
    await context.read<LeaguesProvider>().reload();
  }

  @override
  Widget build(BuildContext context) {
    if (_auth.status != AuthStatus.authenticated) {
      return const ArcadePage(
        centerBody: true,
        maxWidth: 480,
        body: _SignedOutLeagues(),
      );
    }

    final leagues = context.watch<LeaguesProvider>();
    final text = Theme.of(context).textTheme;

    return ArcadePage(
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            children: [
              Text('My Leagues', style: text.titleLarge),
              const SizedBox(width: 12),
              Expanded(
                child: Wrap(
                  alignment: WrapAlignment.end,
                  spacing: 8,
                  runSpacing: 8,
                  children: [
                    FilledButton.tonal(
                      onPressed: () =>
                          _openAndReload(const CreateLeagueScreen()),
                      child: const Text('Create League'),
                    ),
                    FilledButton.tonal(
                      onPressed: () => _openAndReload(const JoinLeagueScreen()),
                      child: const Text('Join with code'),
                    ),
                  ],
                ),
              ),
            ],
          ),
          const SizedBox(height: 16),
          Expanded(
            child: leagues.loading
                ? const Center(child: CircularProgressIndicator())
                : Column(
                    children: [
                      if (leagues.error != null)
                        StatusBanner(text: leagues.error!),
                      Expanded(
                        child: leagues.leagues.isEmpty
                            ? Center(
                                child: Text(
                                  'No leagues yet. Create one or join with a code.',
                                  style: text.bodyLarge?.copyWith(
                                    color: ArcadeColors.creamMuted,
                                  ),
                                  textAlign: TextAlign.center,
                                ),
                              )
                            : ListView.builder(
                                itemCount: leagues.leagues.length,
                                itemBuilder: (context, index) {
                                  final league = leagues.leagues[index];
                                  return Card(
                                    child: ListTile(
                                      leading: Image.asset(
                                        ArcadeAssets.helmet,
                                        width: 40,
                                        height: 40,
                                        filterQuality: FilterQuality.none,
                                      ),
                                      title: Text(league.name),
                                      subtitle: Text(league.joinCode),
                                      trailing: Column(
                                        mainAxisAlignment:
                                            MainAxisAlignment.center,
                                        crossAxisAlignment:
                                            CrossAxisAlignment.end,
                                        children: [
                                          Text(league.role.label),
                                          Text(league.scoring.label),
                                        ],
                                      ),
                                      onTap: () {
                                        final api = context.read<LeagueApi>();
                                        Navigator.of(context).push(
                                          MaterialPageRoute<void>(
                                            builder: (_) =>
                                                LeagueInformationScreen(
                                                  leagueId: league.leagueId,
                                                  leagueApi: api,
                                                ),
                                          ),
                                        );
                                      },
                                    ),
                                  );
                                },
                              ),
                      ),
                    ],
                  ),
          ),
        ],
      ),
    );
  }
}

class _SignedOutLeagues extends StatelessWidget {
  const _SignedOutLeagues();

  @override
  Widget build(BuildContext context) {
    final text = Theme.of(context).textTheme;

    return PixelPanel(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Image.asset(
            ArcadeAssets.helmet,
            height: 88,
            filterQuality: FilterQuality.none,
          ),
          const SizedBox(height: 20),
          Text('Leagues', style: text.headlineMedium),
          const SizedBox(height: 12),
          Text(
            'Sign in to run your franchise.',
            textAlign: TextAlign.center,
            style: text.bodyLarge?.copyWith(color: ArcadeColors.creamMuted),
          ),
          const SizedBox(height: 24),
          FilledButton(
            onPressed: () {
              context.read<NavigationController>().go(
                AppSection.login,
                context,
              );
            },
            child: const Text('Sign in'),
          ),
        ],
      ),
    );
  }
}
