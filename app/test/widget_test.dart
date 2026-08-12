import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/app.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/auth_service.dart';
import 'package:football_gm_app/auth/token_store.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/core/network/api_client.dart';
import 'package:football_gm_app/core/network/api_service.dart';
import 'package:football_gm_app/data/db_provider.dart';
import 'package:football_gm_app/data/team_repository.dart';

void main() {
  testWidgets('Signed-out users see the login screen', (tester) async {
    final tokenStore = TokenStore();
    final apiClient = ApiClient(
      baseUrl: ApiConfig.baseUrl,
      tokenStore: tokenStore,
    );
    final authService = AuthService(
      apiClient: apiClient,
      tokenStore: tokenStore,
    );
    final authController = AuthController(authService: authService)
      ..status = AuthStatus.unauthenticated;

    await tester.pumpWidget(
      FootballGmApp(
        authController: authController,
        authService: authService,
        teamRepository: TeamRepository(
          apiService: ApiService.fromClient(apiClient),
          dbProvider: DbProvider(),
        ),
      ),
    );
    await tester.pump();

    expect(find.text('Sign in'), findsWidgets);
    expect(find.text('Create an account'), findsOneWidget);
    expect(find.text('Sync'), findsNothing);
  });
}
