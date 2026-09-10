import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/features/draft/draft_hub_client.dart';
import 'package:football_gm_app/features/draft/screens/draft_playground_screen.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/ui.dart';
import 'package:provider/provider.dart';

import 'logged_in_auth.dart';

// PLAYGROUND: delete this file.
void main() {
  testWidgets('Playground shows the hub counter and +/- invoke the client', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.counterController.close);
    final auth = loggedInAuth();

    await tester.pumpWidget(
      MultiProvider(
        providers: [
          ChangeNotifierProvider.value(value: auth.controller),
          ChangeNotifierProvider(create: (_) => NavigationController()),
        ],
        child: MaterialApp(
          theme: ArcadeTheme.dark(),
          home: DraftPlaygroundScreen(client: client),
        ),
      ),
    );
    await tester.pump();

    expect(client.connectedDraftId, 'playground');
    expect(find.text('SignalR playground'), findsOneWidget);

    client.counterController.add(0);
    await tester.pump();
    expect(find.text('0'), findsOneWidget);

    await tester.tap(find.text('+'));
    await tester.pump();
    expect(client.incrementCalls, 1);

    client.counterController.add(4);
    await tester.pump();
    expect(find.text('4'), findsOneWidget);

    await tester.tap(find.text('−'));
    await tester.pump();
    expect(client.decrementCalls, 1);
  });
}

class _FakeDraftHubClient implements DraftHubClient {
  final counterController = StreamController<int>.broadcast();
  String? connectedDraftId;
  var incrementCalls = 0;
  var decrementCalls = 0;

  @override
  Stream<int> get counter => counterController.stream;

  @override
  Future<void> connect({required String draftId}) async {
    connectedDraftId = draftId;
    counterController.add(0);
  }

  @override
  Future<void> increment() async {
    incrementCalls++;
  }

  @override
  Future<void> decrement() async {
    decrementCalls++;
  }

  @override
  Future<void> disconnect() async {}
}
