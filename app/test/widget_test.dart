import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/main.dart';

void main() {
  testWidgets('Home page shows Football GM title and empty teams state', (WidgetTester tester) async {
    await tester.pumpWidget(const MyApp());
    await tester.pump();

    expect(find.text('Football GM'), findsOneWidget);
    expect(find.text('Teams'), findsOneWidget);
    expect(find.text('No teams found'), findsOneWidget);
    expect(find.text('Sync'), findsOneWidget);
  });
}
