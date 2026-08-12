import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/config/api_config.dart';
import 'package:football_gm_app/data/db_provider.dart';
import 'package:football_gm_app/main.dart';
import 'package:football_gm_app/repositories/repository.dart';
import 'package:football_gm_app/services/api_client.dart';
import 'package:football_gm_app/services/api_service.dart';
import 'package:football_gm_app/services/auth_service.dart';
import 'package:football_gm_app/services/token_store.dart';

void main() {
  testWidgets('Home page shows Football GM title and empty teams state', (WidgetTester tester) async {
    final tokenStore = TokenStore();
    final apiClient = ApiClient(
      baseUrl: ApiConfig.baseUrl,
      tokenStore: tokenStore,
    );
    final authService = AuthService(
      apiClient: apiClient,
      tokenStore: tokenStore,
    );
    final repository = Repository(
      apiService: ApiService.fromClient(apiClient),
      dbProvider: DbProvider(),
    );

    await tester.pumpWidget(
      MyApp(
        authService: authService,
        repository: repository,
      ),
    );
    await tester.pump();

    expect(find.text('Football GM'), findsOneWidget);
    expect(find.text('Teams'), findsOneWidget);
    expect(find.text('No teams found'), findsOneWidget);
    expect(find.text('Sync'), findsOneWidget);
  });
}
