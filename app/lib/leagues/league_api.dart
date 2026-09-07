import 'package:dio/dio.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

/// List of Leagues the signed-in User belongs to.
abstract class LeagueApi {
  Future<List<LeagueSummary>> listMyLeagues();
}

class HttpLeagueApi implements LeagueApi {
  HttpLeagueApi({required Dio this._dio});

  final Dio _dio;

  @override
  Future<List<LeagueSummary>> listMyLeagues() async {
    final resp = await _dio.get<List<dynamic>>('/api/league');
    if (resp.statusCode == 200 && resp.data != null) {
      return resp.data!
          .map(
            (e) => LeagueSummary.fromJson(Map<String, dynamic>.from(e as Map)),
          )
          .toList();
    }
    throw Exception('Failed to load leagues: ${resp.statusCode}');
  }
}
