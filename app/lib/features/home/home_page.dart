import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/screens/change_password_screen.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/leagues_provider.dart';
import 'package:football_gm_app/leagues/screens/create_league_screen.dart';
import 'package:football_gm_app/leagues/screens/join_league_screen.dart';
import 'package:football_gm_app/leagues/screens/league_information_screen.dart';
import 'package:provider/provider.dart';

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      context.read<LeaguesProvider>().reload();
    });
  }

  @override
  Widget build(BuildContext context) {
    final leagues = context.watch<LeaguesProvider>();
    final auth = context.watch<AuthController>();
    final name = auth.user?.displayName ?? 'GM';

    return Scaffold(
      appBar: AppBar(
        title: const Text('Football GM'),
        actions: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8),
            child: Center(child: Text(name)),
          ),
          PopupMenuButton<_Menu>(
            tooltip: 'Account',
            enabled: !auth.busy,
            onSelected: (item) async {
              switch (item) {
                case _Menu.changePassword:
                  auth.clearError();
                  await Navigator.of(context).push(
                    MaterialPageRoute<void>(
                      builder: (_) => const ChangePasswordScreen(),
                    ),
                  );
                case _Menu.signOut:
                  await auth.logout();
              }
            },
            itemBuilder: (_) => const [
              PopupMenuItem(
                value: _Menu.changePassword,
                child: Text('Change password'),
              ),
              PopupMenuItem(value: _Menu.signOut, child: Text('Sign out')),
            ],
          ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(8),
            child: Row(
              children: [
                Text(
                  'My Leagues',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const Spacer(),
                FilledButton.tonal(
                  onPressed: () async {
                    await Navigator.of(context).push(
                      MaterialPageRoute<void>(
                        builder: (_) => const CreateLeagueScreen(),
                      ),
                    );
                    if (!context.mounted) return;
                    await context.read<LeaguesProvider>().reload();
                  },
                  child: const Text('Create League'),
                ),
                const SizedBox(width: 8),
                FilledButton.tonal(
                  onPressed: () async {
                    await Navigator.of(context).push(
                      MaterialPageRoute<void>(
                        builder: (_) => const JoinLeagueScreen(),
                      ),
                    );
                    if (!context.mounted) return;
                    await context.read<LeaguesProvider>().reload();
                  },
                  child: const Text('Join with code'),
                ),
              ],
            ),
          ),
          Expanded(
            child: leagues.loading
                ? const Center(child: CircularProgressIndicator())
                : Column(
                    children: [
                      if (leagues.error != null)
                        Padding(
                          padding: const EdgeInsets.all(8),
                          child: Text(
                            leagues.error!,
                            style: TextStyle(
                              color: Theme.of(context).colorScheme.error,
                            ),
                          ),
                        ),
                      Expanded(
                        child: ListView.builder(
                          itemCount: leagues.leagues.length,
                          itemBuilder: (context, index) {
                            final league = leagues.leagues[index];
                            return ListTile(
                              title: Text(league.name),
                              subtitle: Text(league.joinCode),
                              trailing: Column(
                                mainAxisAlignment: MainAxisAlignment.center,
                                crossAxisAlignment: CrossAxisAlignment.end,
                                children: [
                                  Text(league.role.label),
                                  Text(league.scoring.label),
                                ],
                              ),
                              onTap: () {
                                final api = context.read<LeagueApi>();
                                Navigator.of(context).push(
                                  MaterialPageRoute<void>(
                                    builder: (_) => LeagueInformationScreen(
                                      leagueId: league.leagueId,
                                      leagueApi: api,
                                    ),
                                  ),
                                );
                              },
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

enum _Menu { changePassword, signOut }
