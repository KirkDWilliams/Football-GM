import 'package:football_gm_app/core/network/api_service.dart';
import 'package:football_gm_app/data/db_provider.dart';
import 'package:football_gm_app/models/team.dart';

/// Loads teams from the API and optional local cache.
class TeamRepository {
  TeamRepository({
    required this.apiService,
    required this.dbProvider,
  });

  final ApiService apiService;
  final DbProvider dbProvider;

  Future<void> syncTeams() async {
    final teams = await apiService.getTeams();
    for (final t in teams) {
      await dbProvider.insertTeam(t);
    }
  }

  Future<List<Team>> getLocalTeams() => dbProvider.getTeams();
}
