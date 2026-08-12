import 'package:flutter/foundation.dart';
import 'package:football_gm_app/data/team_repository.dart';
import 'package:football_gm_app/models/team.dart';

class TeamsProvider extends ChangeNotifier {
  TeamsProvider(this.repository) {
    loadLocal();
  }

  final TeamRepository repository;
  List<Team> teams = [];
  bool loading = false;

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
      // Surface errors when teams features are built out.
    }
    loading = false;
    notifyListeners();
  }
}
