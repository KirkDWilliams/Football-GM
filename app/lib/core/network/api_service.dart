import 'package:dio/dio.dart';
import 'package:football_gm_app/core/network/api_client.dart';
import 'package:football_gm_app/models/team.dart';

/// Non-auth API calls (uses the shared authenticated Dio).
class ApiService {
  ApiService({required Dio dio}) : _dio = dio;

  factory ApiService.fromClient(ApiClient client) =>
      ApiService(dio: client.dio);

  final Dio _dio;

  Future<List<Team>> getTeams() async {
    final resp = await _dio.get<List<dynamic>>('/api/teams');
    if (resp.statusCode == 200 && resp.data != null) {
      return resp.data!
          .map((e) => Team.fromJson(Map<String, dynamic>.from(e as Map)))
          .toList();
    }
    throw Exception('Failed to load teams: ${resp.statusCode}');
  }

  Future<bool> healthCheck() async {
    final resp = await _dio.get<dynamic>('/api/health');
    return resp.statusCode == 200;
  }
}
