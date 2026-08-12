import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/auth/models/auth_session.dart';

void main() {
  test('AuthSession.fromJson parses API payload', () {
    final session = AuthSession.fromJson({
      'accessToken': 'access-abc',
      'tokenType': 'Bearer',
      'expiresAt': '2026-08-11T12:00:00+00:00',
      'refreshToken': 'refresh-xyz',
      'refreshExpiresAt': '2026-09-10T12:00:00+00:00',
      'user': {
        'id': 'user1',
        'email': 'gm@example.com',
        'displayName': 'Nick',
      },
    });

    expect(session.accessToken, 'access-abc');
    expect(session.user.email, 'gm@example.com');
  });
}
