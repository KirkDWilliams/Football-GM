import 'package:dio/dio.dart';

/// API auth/validation error with a user-facing message when available.
class AuthException implements Exception {
  AuthException(this.message, {this.statusCode});

  final String message;
  final int? statusCode;

  factory AuthException.fromDio(DioException error) {
    final status = error.response?.statusCode;
    final data = error.response?.data;

    if (data is Map && data['error'] is String) {
      return AuthException(data['error'] as String, statusCode: status);
    }

    if (status == 401) {
      return AuthException('Unauthorized', statusCode: status);
    }
    if (status == 409) {
      return AuthException('Conflict', statusCode: status);
    }

    return AuthException(
      error.message ?? 'Request failed',
      statusCode: status,
    );
  }

  @override
  String toString() => message;
}
