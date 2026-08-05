import 'package:dio/dio.dart';
import '../models/team.dart';

class ApiService {
  final String baseUrl;
  final Dio _dio;

  ApiService({required this.baseUrl})
      : _dio = Dio(BaseOptions(
          baseUrl: baseUrl,
          connectTimeout: const Duration(seconds: 10),
          receiveTimeout: const Duration(seconds: 15),
        ));

  Future<List<Team>> getTeams() async {
    final resp = await _dio.get('/api/teams');
    if (resp.statusCode == 200) {
      final data = resp.data as List<dynamic>;
      return data.map((e) => Team.fromJson(Map<String, dynamic>.from(e as Map))).toList();
    }
    throw Exception('Failed to load teams: ${resp.statusCode}');
  }

  Future<bool> healthCheck() async {
    final resp = await _dio.get('/api/health');
    return resp.statusCode == 200;
  }
}
