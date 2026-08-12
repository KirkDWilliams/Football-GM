import 'package:dio/dio.dart';

import '../models/team.dart';
import 'api_client.dart';

/// Domain API calls (teams, etc.) using the shared authenticated [Dio] client.
class ApiService {
  ApiService({required Dio dio}) : _dio = dio;

  /// Convenience constructor for apps that already built an [ApiClient].
  factory ApiService.fromClient(ApiClient client) => ApiService(dio: client.dio);

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
