import 'package:flutter/foundation.dart';
import 'package:football_gm_app/models/player.dart';
import 'package:football_gm_app/models/bid.dart';

class DraftProvider extends ChangeNotifier {
  DraftProvider(this._api);

  final HttpDraftApi _api;
  List<Player> players = [];
  bool loading = true;
  String? error;

  Future<void> reload() async {
    loading = true;
    error = null;
    notifyListeners();
    try 
    {
      players = await _api.getAvailablePlayers();
    } catch (_) {
      error = 'Could not load Players';
    }
    loading = false;
    notifyListeners();
  }
}
