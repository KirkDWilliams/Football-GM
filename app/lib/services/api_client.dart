import 'dart:async';

import 'package:dio/dio.dart';

import '../models/auth_session.dart';
import 'token_store.dart';

/// Shared [Dio] with Bearer attachment and single-flight refresh on 401.
class ApiClient {
  ApiClient({
    required String baseUrl,
    required TokenStore tokenStore,
  })  : _tokenStore = tokenStore,
        _dio = Dio(
          BaseOptions(
            baseUrl: baseUrl,
            connectTimeout: const Duration(seconds: 10),
            receiveTimeout: const Duration(seconds: 15),
            headers: {
              'Content-Type': 'application/json',
              'Accept': 'application/json',
            },
          ),
        ) {
    // Separate client without interceptors so refresh cannot recurse.
    _refreshDio = Dio(
      BaseOptions(
        baseUrl: baseUrl,
        connectTimeout: const Duration(seconds: 10),
        receiveTimeout: const Duration(seconds: 15),
        headers: {'Content-Type': 'application/json', 'Accept': 'application/json'},
      ),
    );

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: _onRequest,
        onError: _onError,
      ),
    );
  }

  final TokenStore _tokenStore;
  final Dio _dio;
  late final Dio _refreshDio;

  /// In-flight refresh so concurrent 401s share one refresh call.
  Future<AuthSession?>? _refreshInFlight;

  Dio get dio => _dio;

  void _onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) {
    if (_shouldSkipAuth(options.path)) {
      handler.next(options);
      return;
    }

    final token = _tokenStore.accessToken;
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }

    handler.next(options);
  }

  Future<void> _onError(
    DioException error,
    ErrorInterceptorHandler handler,
  ) async {
    final response = error.response;
    final request = error.requestOptions;

    if (response?.statusCode != 401 ||
        _shouldSkipAuth(request.path) ||
        request.extra['authRetried'] == true) {
      handler.next(error);
      return;
    }

    try {
      final session = await _refreshSession();
      if (session == null) {
        handler.next(error);
        return;
      }

      final opts = request.copyWith(
        extra: {...request.extra, 'authRetried': true},
        headers: Map<String, dynamic>.from(request.headers)
          ..['Authorization'] = 'Bearer ${session.accessToken}',
      );

      final clone = await _dio.fetch(opts);
      handler.resolve(clone);
    } on Object {
      handler.next(error);
    }
  }

  /// Uses the stored refresh token; updates [TokenStore] on success.
  Future<AuthSession?> refreshSession() => _refreshSession();

  Future<AuthSession?> _refreshSession() async {
    if (_refreshInFlight != null) {
      return _refreshInFlight;
    }

    final completer = Completer<AuthSession?>();
    _refreshInFlight = completer.future;

    try {
      final refresh = _tokenStore.refreshToken;
      if (refresh == null || refresh.isEmpty) {
        await _tokenStore.clear();
        completer.complete(null);
        return null;
      }

      final response = await _refreshDio.post<Map<String, dynamic>>(
        '/api/auth/refresh',
        data: {'refreshToken': refresh},
      );

      if (response.statusCode != 200 || response.data == null) {
        await _tokenStore.clear();
        completer.complete(null);
        return null;
      }

      final session = AuthSession.fromJson(response.data!);
      await _tokenStore.save(session);
      completer.complete(session);
      return session;
    } on Object {
      await _tokenStore.clear();
      completer.complete(null);
      return null;
    } finally {
      _refreshInFlight = null;
    }
  }

  static bool _shouldSkipAuth(String path) {
    final p = path.toLowerCase();
    return p.contains('/api/auth/login') ||
        p.contains('/api/auth/register') ||
        p.contains('/api/auth/refresh') ||
        p.contains('/api/auth/logout') ||
        p.contains('/api/health');
  }
}
