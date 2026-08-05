import 'package:flutter/foundation.dart';

/// Central API connection settings for local development.
///
/// Override at runtime with:
/// `flutter run --dart-define=API_BASE_URL=http://192.168.1.10:5000`
class ApiConfig {
  static const String _definedBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
    defaultValue: '',
  );

  /// Resolved API base URL for the current platform.
  static String get baseUrl {
    if (_definedBaseUrl.isNotEmpty) {
      return _definedBaseUrl;
    }

    // Android emulator reaches the host machine via 10.0.2.2
    if (!kIsWeb && defaultTargetPlatform == TargetPlatform.android) {
      return 'http://10.0.2.2:5000';
    }

    // Windows desktop, web, and other host platforms
    return 'http://localhost:5000';
  }
}
