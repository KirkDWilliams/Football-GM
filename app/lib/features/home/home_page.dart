import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/screens/change_password_screen.dart';
import 'package:football_gm_app/features/home/teams_provider.dart';
import 'package:provider/provider.dart';

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final teams = context.watch<TeamsProvider>();
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
              PopupMenuItem(
                value: _Menu.signOut,
                child: Text('Sign out'),
              ),
            ],
          ),
        ],
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Teams',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                FilledButton.tonalIcon(
                  onPressed: teams.loading ? null : teams.sync,
                  icon: teams.loading
                      ? const SizedBox(
                          width: 16,
                          height: 16,
                          child: CircularProgressIndicator(strokeWidth: 2),
                        )
                      : const Icon(Icons.sync),
                  label: const Text('Sync'),
                ),
              ],
            ),
          ),
          Expanded(
            child: teams.teams.isEmpty
                ? const Center(child: Text('No teams found'))
                : ListView.builder(
                    itemCount: teams.teams.length,
                    itemBuilder: (context, index) {
                      final t = teams.teams[index];
                      return ListTile(
                        title: Text(t.name),
                        subtitle: Text(t.city),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }
}

enum _Menu { changePassword, signOut }
