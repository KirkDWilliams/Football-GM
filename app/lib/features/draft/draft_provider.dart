import 'package:flutter/foundation.dart';
import 'package:football_gm_app/features/draft/draft_api.dart';
import 'package:football_gm_app/models/player.dart';

class DraftProvider extends ChangeNotifier {
  DraftProvider(this._api, {required this.leagueId});

  final DraftApi _api;
  final int leagueId;
  List<Player> players = [];
  bool loading = true;
  String? error;

  Future<void> reload() async {
    loading = true;
    error = null;
    notifyListeners();
    try 
    {
      players = await _api.getAvailablePlayers(leagueId);
    } catch (_) {
      error = 'Could not load Players';
    }
    loading = false;
    notifyListeners();
  }
}
