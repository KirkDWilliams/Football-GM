import 'package:dio/dio.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/models/league_summary.dart';

/// Leagues the signed-in User belongs to.
abstract class LeagueApi {
  Future<List<LeagueSummary>> listMyLeagues();
  Future<LeagueDetails> getLeague(int leagueId);

  /// Creates a League and returns its id for the information screen to load.
  Future<int> createLeague({required String name, required num weeklyCap});

  /// Joins a League by Join code and returns its id. Already a Member is success.
  Future<int> joinLeague(String joinCode);
}

class UnknownJoinCodeException implements Exception {}

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

  @override
  Future<int> createLeague({
    required String name,
    required num weeklyCap,
  }) async {
    final resp = await _dio.post<Map<String, dynamic>>(
      '/api/league',
      data: {'name': name, 'weeklyCapSpace': weeklyCap},
    );
    if (resp.statusCode == 201 && resp.data != null) {
      return resp.data!['leagueId'] as int;
    }
    throw Exception('Failed to create league: ${resp.statusCode}');
  }

  @override
  Future<int> joinLeague(String joinCode) async {
    try {
      final resp = await _dio.post<Map<String, dynamic>>(
        '/api/league/$joinCode',
      );
      return _leagueIdFrom(resp.data, resp.statusCode);
    } on DioException catch (error) {
      if (error.response?.statusCode == 409) {
        return _leagueIdFrom(error.response?.data, 409);
      }
      if (error.response?.statusCode == 404) {
        throw UnknownJoinCodeException();
      }
      rethrow;
    }
  }

  static int _leagueIdFrom(dynamic data, int? statusCode) {
    if (data is Map && data['leagueId'] is int) {
      return data['leagueId'] as int;
    }
    throw Exception('Failed to join league: $statusCode');
  }
}
