import 'package:dio/dio.dart';
import 'package:football_gm_app/auth/auth_exception.dart';
import 'package:football_gm_app/auth/models/auth_session.dart';
import 'package:football_gm_app/auth/models/auth_user.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/core/network/api_client.dart';

/// Calls the auth API and keeps [TokenStore] in sync.
class AuthService {
  AuthService({
    required ApiClient apiClient,
    required TokenStore tokenStore,
  })  : _apiClient = apiClient,
        _tokenStore = tokenStore,
        _dio = apiClient.dio;

  final ApiClient _apiClient;
  final TokenStore _tokenStore;
  final Dio _dio;

  TokenStore get tokenStore => _tokenStore;
  AuthSession? get currentSession => _tokenStore.session;

  Future<AuthSession> register({
    required String email,
    required String password,
    required String displayName,
    String? deviceName,
  }) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        '/api/auth/register',
        data: {
          'email': email,
          'password': password,
          'displayName': displayName,
          if (deviceName != null) 'deviceName': deviceName,
        },
      );

      if (response.statusCode != 201 || response.data == null) {
        throw AuthException(
          'Registration failed',
          statusCode: response.statusCode,
        );
      }

      final session = AuthSession.fromJson(response.data!);
      await _tokenStore.save(session);
      return session;
    } on DioException catch (e) {
      throw AuthException.fromDio(e);
    }
  }

  Future<AuthSession> login({
    required String email,
    required String password,
    String? deviceName,
  }) async {
    try {
      final response = await _dio.post<Map<String, dynamic>>(
        '/api/auth/login',
        data: {
          'email': email,
          'password': password,
          if (deviceName != null) 'deviceName': deviceName,
        },
      );

      if (response.statusCode != 200 || response.data == null) {
        throw AuthException('Login failed', statusCode: response.statusCode);
      }

      final session = AuthSession.fromJson(response.data!);
      await _tokenStore.save(session);
      return session;
    } on DioException catch (e) {
      throw AuthException.fromDio(e);
    }
  }

  Future<AuthSession?> refresh() => _apiClient.refreshSession();

  Future<void> logout() async {
    final refresh = _tokenStore.refreshToken;
    try {
      if (refresh != null && refresh.isNotEmpty) {
        await _dio.post<void>(
          '/api/auth/logout',
          data: {'refreshToken': refresh},
        );
      }
    } on DioException {
      // Always clear local session.
    } finally {
      await _tokenStore.clear();
    }
  }

  Future<AuthUser> me() async {
    try {
      final response = await _dio.get<Map<String, dynamic>>('/api/auth/me');
      if (response.statusCode != 200 || response.data == null) {
        throw AuthException(
          'Failed to load profile',
          statusCode: response.statusCode,
        );
      }
      return AuthUser.fromJson(response.data!);
    } on DioException catch (e) {
      throw AuthException.fromDio(e);
    }
  }

  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    try {
      final response = await _dio.post<void>(
        '/api/auth/change-password',
        data: {
          'currentPassword': currentPassword,
          'newPassword': newPassword,
        },
      );

      if (response.statusCode != 204 && response.statusCode != 200) {
        throw AuthException(
          'Change password failed',
          statusCode: response.statusCode,
        );
      }

      await _tokenStore.clear();
    } on DioException catch (e) {
      throw AuthException.fromDio(e);
    }
  }
}
