import '../services/api_service.dart';
import '../data/db_provider.dart';
import '../models/team.dart';

class Repository {
  final ApiService apiService;
  final DbProvider dbProvider;

  Repository({required this.apiService, required this.dbProvider});

  /// Fetch teams from remote API and upsert into local DB.
  Future<void> syncTeams() async {
	final teams = await apiService.getTeams();
	for (final t in teams) {
	  await dbProvider.insertTeam(t);
	}
  }

  Future<List<Team>> getLocalTeams() async {
	return await dbProvider.getTeams();
  }
}
