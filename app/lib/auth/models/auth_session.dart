import 'auth_user.dart';

/// Access + refresh tokens and user from register/login/refresh.
class AuthSession {
  const AuthSession({
    required this.accessToken,
    required this.tokenType,
    required this.expiresAt,
    required this.refreshToken,
    required this.refreshExpiresAt,
    required this.user,
  });

  final String accessToken;
  final String tokenType;
  final DateTime expiresAt;
  final String refreshToken;
  final DateTime refreshExpiresAt;
  final AuthUser user;

  bool get isAccessExpired =>
      DateTime.now().toUtc().isAfter(expiresAt.toUtc());

  bool get isRefreshExpired =>
      DateTime.now().toUtc().isAfter(refreshExpiresAt.toUtc());

  factory AuthSession.fromJson(Map<String, dynamic> json) {
    final userJson = json['user'];
    if (userJson is! Map) {
      throw const FormatException('Auth response missing user object');
    }

    return AuthSession(
      accessToken: json['accessToken'] as String,
      tokenType: (json['tokenType'] as String?) ?? 'Bearer',
      expiresAt: DateTime.parse(json['expiresAt'] as String).toUtc(),
      refreshToken: json['refreshToken'] as String,
      refreshExpiresAt:
          DateTime.parse(json['refreshExpiresAt'] as String).toUtc(),
      user: AuthUser.fromJson(Map<String, dynamic>.from(userJson)),
    );
  }

  Map<String, dynamic> toJson() => {
        'accessToken': accessToken,
        'tokenType': tokenType,
        'expiresAt': expiresAt.toUtc().toIso8601String(),
        'refreshToken': refreshToken,
        'refreshExpiresAt': refreshExpiresAt.toUtc().toIso8601String(),
        'user': user.toJson(),
      };
}
