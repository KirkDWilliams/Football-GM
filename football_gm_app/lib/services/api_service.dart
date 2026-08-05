import 'package:dio/dio.dart';
import '../models/team.dart';

class ApiService {
  // TODO: Set your backend base URL here, e.g. 'http://10.0.2.2:5000' for Android emulator
  final String baseUrl;
  final Dio _dio;

  ApiService({required this.baseUrl}) : _dio = Dio(BaseOptions(baseUrl: baseUrl));

  Future<List<Team>> getTeams() async {
	final resp = await _dio.get('/api/teams');
	if (resp.statusCode == 200) {
	  final data = resp.data as List<dynamic>;
	  return data.map((e) => Team.fromJson(Map<String, dynamic>.from(e))).toList();
	}
	throw Exception('Failed to load teams: ${resp.statusCode}');
  }
}
