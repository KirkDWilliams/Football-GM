import 'dart:convert';

import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import '../models/auth_session.dart';

/// Persists the current auth session (access + refresh + user) on device.
///
/// Uses platform secure storage where available. Do not log token values.
class TokenStore {
  TokenStore({FlutterSecureStorage? storage})
      : _storage = storage ??
            const FlutterSecureStorage(
              aOptions: AndroidOptions(encryptedSharedPreferences: true),
            );

  static const _sessionKey = 'football_gm.auth_session';

  final FlutterSecureStorage _storage;

  /// In-memory cache so interceptors do not hit disk on every request.
  AuthSession? _cached;

  AuthSession? get session => _cached;

  String? get accessToken => _cached?.accessToken;

  String? get refreshToken => _cached?.refreshToken;

  Future<AuthSession?> load() async {
    final raw = await _storage.read(key: _sessionKey);
    if (raw == null || raw.isEmpty) {
      _cached = null;
      return null;
    }

    try {
      final map = jsonDecode(raw) as Map<String, dynamic>;
      _cached = AuthSession.fromJson(map);
      return _cached;
    } on Object {
      await clear();
      return null;
    }
  }

  Future<void> save(AuthSession session) async {
    _cached = session;
    await _storage.write(
      key: _sessionKey,
      value: jsonEncode(session.toJson()),
    );
  }

  Future<void> clear() async {
    _cached = null;
    await _storage.delete(key: _sessionKey);
  }
}
