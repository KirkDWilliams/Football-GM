import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/services/api_service.dart';
import 'package:football_gm_app/data/db_provider.dart';
import 'package:football_gm_app/repositories/repository.dart';
import 'package:football_gm_app/models/team.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    final apiService = ApiService(baseUrl: ApiConfig.baseUrl);
    final dbProvider = DbProvider();
    final repository = Repository(apiService: apiService, dbProvider: dbProvider);

    return MultiProvider(
      providers: [
        Provider<Repository>.value(value: repository),
        ChangeNotifierProvider<TeamsProvider>(create: (_) => TeamsProvider(repository)),
      ],
      child: MaterialApp(
        title: 'Football GM',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
          useMaterial3: true,
        ),
        home: const HomePage(),
      ),
    );
  }
}

class TeamsProvider extends ChangeNotifier {
  final Repository repository;
  List<Team> teams = [];
  bool loading = false;

  TeamsProvider(this.repository) {
    loadLocal();
  }

  Future<void> loadLocal() async {
    try {
      teams = await repository.getLocalTeams();
    } catch (_) {
      teams = [];
    }
    notifyListeners();
  }

  Future<void> sync() async {
    loading = true;
    notifyListeners();
    try {
      await repository.syncTeams();
      await loadLocal();
    } catch (_) {
      // Error state will be surfaced when feature work begins.
    }
    loading = false;
    notifyListeners();
  }
}

class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final provider = Provider.of<TeamsProvider>(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Football GM')),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text('Teams', style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
                ElevatedButton.icon(
                  onPressed: provider.loading ? null : () => provider.sync(),
                  icon: provider.loading
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
            child: provider.teams.isEmpty
                ? const Center(child: Text('No teams found'))
                : ListView.builder(
                    itemCount: provider.teams.length,
                    itemBuilder: (context, index) {
                      final t = provider.teams[index];
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
