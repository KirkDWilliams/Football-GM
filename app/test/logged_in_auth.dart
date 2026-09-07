import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/models/auth_user.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/core/network/api_client.dart';

({AuthController controller, AuthService service}) loggedInAuth({
  AuthStatus status = AuthStatus.authenticated,
}) {
  final tokenStore = TokenStore();
  final apiClient = ApiClient(
    baseUrl: ApiConfig.baseUrl,
    tokenStore: tokenStore,
  );
  final service = AuthService(apiClient: apiClient, tokenStore: tokenStore);
  final controller = AuthController(authService: service)..status = status;
  if (status == AuthStatus.authenticated) {
    controller.user = const AuthUser(
      id: 'user-1',
      email: 'gm@example.com',
      displayName: 'Nick',
    );
  }
  return (controller: controller, service: service);
}
