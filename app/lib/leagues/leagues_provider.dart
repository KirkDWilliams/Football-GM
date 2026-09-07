import 'package:flutter/foundation.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

class LeaguesProvider extends ChangeNotifier {
  LeaguesProvider(this._api);

  final LeagueApi _api;
  List<LeagueSummary> leagues = [];
  bool loading = true;

  Future<void> reload() async {
    loading = true;
    notifyListeners();
    try {
      leagues = await _api.listMyLeagues();
    } catch (_) {
      leagues = [];
    }
    loading = false;
    notifyListeners();
  }
}
