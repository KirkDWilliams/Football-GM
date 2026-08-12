import 'package:flutter/foundation.dart';
import 'package:football_gm_app/auth/auth_exception.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/models/auth_user.dart';

enum AuthStatus {
  unknown,
  authenticated,
  unauthenticated,
}

/// UI-facing auth state (gate, forms, account menu).
class AuthController extends ChangeNotifier {
  AuthController({required AuthService authService}) : _auth = authService;

  final AuthService _auth;

  AuthStatus status = AuthStatus.unknown;
  bool busy = false;
  String? errorMessage;
  String? successMessage;
  AuthUser? user;

  String get _deviceName =>
      kIsWeb ? 'flutter-web' : 'flutter-${defaultTargetPlatform.name}';

  Future<void> bootstrap() async {
    status = AuthStatus.unknown;
    errorMessage = null;
    notifyListeners();

    if (_auth.currentSession == null) {
      user = null;
      status = AuthStatus.unauthenticated;
      notifyListeners();
      return;
    }

    try {
      user = await _auth.me();
      status = AuthStatus.authenticated;
    } on Object {
      await _auth.tokenStore.clear();
      user = null;
      status = AuthStatus.unauthenticated;
    }
    notifyListeners();
  }

  Future<bool> login({required String email, required String password}) {
    return _run(() async {
      final session = await _auth.login(
        email: email,
        password: password,
        deviceName: _deviceName,
      );
      user = session.user;
      status = AuthStatus.authenticated;
    });
  }

  Future<bool> register({
    required String email,
    required String password,
    required String displayName,
  }) {
    return _run(() async {
      final session = await _auth.register(
        email: email,
        password: password,
        displayName: displayName,
        deviceName: _deviceName,
      );
      user = session.user;
      status = AuthStatus.authenticated;
    });
  }

  Future<void> logout() async {
    busy = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _auth.logout();
    } finally {
      user = null;
      status = AuthStatus.unauthenticated;
      busy = false;
      notifyListeners();
    }
  }

  Future<bool> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    busy = true;
    errorMessage = null;
    notifyListeners();
    try {
      await _auth.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
      );
      user = null;
      status = AuthStatus.unauthenticated;
      successMessage = 'Password updated. Sign in with your new password.';
      return true;
    } on AuthException catch (e) {
      errorMessage = e.message;
      return false;
    } on Object catch (e) {
      errorMessage = e.toString();
      return false;
    } finally {
      busy = false;
      notifyListeners();
    }
  }

  void clearError() {
    if (errorMessage == null) return;
    errorMessage = null;
    notifyListeners();
  }

  void clearSuccessMessage() {
    if (successMessage == null) return;
    successMessage = null;
    notifyListeners();
  }

  Future<bool> _run(Future<void> Function() action) async {
    busy = true;
    errorMessage = null;
    successMessage = null;
    notifyListeners();
    try {
      await action();
      return true;
    } on AuthException catch (e) {
      errorMessage = e.message;
      status = AuthStatus.unauthenticated;
      return false;
    } on Object catch (e) {
      errorMessage = e.toString();
      status = AuthStatus.unauthenticated;
      return false;
    } finally {
      busy = false;
      notifyListeners();
    }
  }
}
