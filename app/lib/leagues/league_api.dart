import 'package:dio/dio.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

/// Leagues the signed-in User belongs to.
abstract class LeagueApi {
  Future<List<LeagueSummary>> listMyLeagues();
  Future<LeagueDetails> getLeague(int leagueId);
}

class HttpLeagueApi implements LeagueApi {
  HttpLeagueApi({required this._dio});

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

  @override
  Future<LeagueDetails> getLeague(int leagueId) async {
    final resp = await _dio.get<Map<String, dynamic>>('/api/league/$leagueId');
    if (resp.statusCode == 200 && resp.data != null) {
      return LeagueDetails.fromJson(Map<String, dynamic>.from(resp.data!));
    }
    throw Exception('Failed to load league: ${resp.statusCode}');
  }
}
