import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:football_gm_app/features/draft/draft_hub_client.dart';
import 'package:football_gm_app/features/draft/draft_snapshot.dart';
import 'package:football_gm_app/features/draft/screens/draft_room_screen.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/ui.dart';
import 'package:provider/provider.dart';

import 'logged_in_auth.dart';

void main() {
  testWidgets('DraftUpdated replaces the room with that snapshot', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client);
    await _flush(tester);

    expect(client.connectedLeagueId, 3);
    expect(find.text('No open draft'), findsOneWidget);

    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada', 'leftoverFunds': 100},
          {'userId': 'user-2', 'displayName': 'Grace'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Lobby'), findsOneWidget);
    expect(find.text('Ada'), findsOneWidget);
    expect(find.text('Grace'), findsOneWidget);
    expect(find.text('No open draft'), findsNothing);

    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-2', 'displayName': 'Grace'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Ada'), findsNothing);
    expect(find.text('Grace'), findsOneWidget);
    expect(find.text('Lobby'), findsOneWidget);
  });

  testWidgets('Commissioner opens an empty room and the lobby arrives later', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client, isCommissioner: true);
    await _flush(tester);

    expect(find.text('No open draft'), findsOneWidget);
    expect(find.text('Open'), findsOneWidget);
    expect(find.text('Close'), findsNothing);

    await tester.tap(find.text('Open'));
    await _flush(tester);

    expect(client.openCalls, 1);
    expect(find.text('No open draft'), findsOneWidget);
    expect(find.text('Lobby'), findsNothing);

    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada'},
          {'userId': 'user-2', 'displayName': 'Grace'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Lobby'), findsOneWidget);
    expect(find.text('Ada'), findsOneWidget);
    expect(find.text('Grace'), findsOneWidget);
    expect(find.text('Open'), findsNothing);
  });

  testWidgets('A member sees the empty room without command buttons', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client);
    await _flush(tester);

    expect(find.text('No open draft'), findsOneWidget);
    expect(find.text('Open'), findsNothing);
    expect(find.text('Close'), findsNothing);
  });

  testWidgets('Commissioner closes a lobby and the room shows Closed', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client, isCommissioner: true);
    await _flush(tester);
    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada'},
          {'userId': 'user-2', 'displayName': 'Grace'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Close'), findsOneWidget);
    expect(find.text('Open'), findsNothing);

    await tester.tap(find.text('Close'));
    await _flush(tester);

    expect(client.closeCalls, 1);
    expect(find.text('Lobby'), findsOneWidget);
    expect(find.text('Ada'), findsOneWidget);

    client.emit(
      _closed(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada'},
          {'userId': 'user-2', 'displayName': 'Grace'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Closed'), findsOneWidget);
    expect(find.text('Ada'), findsOneWidget);
    expect(find.text('Grace'), findsOneWidget);
    expect(find.text('Close'), findsNothing);
    expect(find.text('Open'), findsOneWidget);
  });

  testWidgets('A rejected Close leaves the lobby and shows the error', (
    tester,
  ) async {
    final client = _FakeDraftHubClient()..failClose = true;
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client, isCommissioner: true);
    await _flush(tester);
    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada'},
        ],
      ),
    );
    await _flush(tester);

    await tester.tap(find.text('Close'));
    await _flush(tester);

    expect(client.closeCalls, 1);
    expect(find.text('Lobby'), findsOneWidget);
    expect(find.text('Ada'), findsOneWidget);
    expect(find.text('Only the commissioner can do that'), findsOneWidget);
    expect(find.text('Closed'), findsNothing);

    client.emit(
      _lobby(
        members: const [
          {'userId': 'user-1', 'displayName': 'Ada'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Only the commissioner can do that'), findsNothing);
    expect(find.text('Lobby'), findsOneWidget);
  });

  testWidgets(
    'Start stays in the lobby until the live snapshot names the nominator',
    (tester) async {
      final client = _FakeDraftHubClient();
      addTearDown(client.updatesController.close);

      await _pumpRoom(tester, client: client, isCommissioner: true);
      await _flush(tester);
      client.emit(
        _lobby(
          members: const [
            {'userId': 'user-1', 'displayName': 'Nick'},
            {'userId': 'user-2', 'displayName': 'Ada'},
          ],
        ),
      );
      await _flush(tester);

      expect(find.text('Start'), findsOneWidget);
      expect(find.text('Close'), findsOneWidget);

      await tester.tap(find.text('Start'));
      await _flush(tester);

      expect(client.startCalls, 1);
      expect(find.text('Lobby'), findsOneWidget);
      expect(find.text('Live'), findsNothing);
      expect(find.text('Up to nominate: Nick'), findsNothing);

      client.emit(
        _snapshot(
          status: 'live',
          currentNominatorUserId: 'user-1',
          members: const [
            {'userId': 'user-1', 'displayName': 'Nick'},
            {'userId': 'user-2', 'displayName': 'Ada'},
          ],
        ),
      );
      await _flush(tester);

      expect(find.text('Live'), findsOneWidget);
      expect(find.text('Up to nominate: Nick'), findsOneWidget);
      expect(find.text('Ada'), findsOneWidget);
      expect(find.text('Start'), findsNothing);
      expect(find.text('Close'), findsNothing);
      expect(find.text('Open'), findsNothing);
    },
  );

  testWidgets('A member sees who nominates and no command buttons', (
    tester,
  ) async {
    final client = _FakeDraftHubClient();
    addTearDown(client.updatesController.close);

    await _pumpRoom(tester, client: client);
    await _flush(tester);
    client.emit(
      _snapshot(
        status: 'live',
        currentNominatorUserId: 'user-1',
        members: const [
          {'userId': 'user-1', 'displayName': 'Nick'},
          {'userId': 'user-2', 'displayName': 'Ada'},
        ],
      ),
    );
    await _flush(tester);

    expect(find.text('Live'), findsOneWidget);
    expect(find.text('Up to nominate: Nick'), findsOneWidget);
    expect(find.text('Start'), findsNothing);
    expect(find.text('Close'), findsNothing);
    expect(find.text('Open'), findsNothing);
  });
}

/// A snapshot landed during idle is drawn on the frame after it is scheduled.
Future<void> _flush(WidgetTester tester) async {
  await tester.pump();
  await tester.pump();
}

Future<void> _pumpRoom(
  WidgetTester tester, {
  required _FakeDraftHubClient client,
  bool isCommissioner = false,
}) async {
  final auth = loggedInAuth();
  await tester.pumpWidget(
    MultiProvider(
      providers: [
        ChangeNotifierProvider.value(value: auth.controller),
        ChangeNotifierProvider(create: (_) => NavigationController()),
      ],
      child: MaterialApp(
        theme: ArcadeTheme.dark(),
        home: DraftRoomScreen(
          client: client,
          leagueId: 3,
          isCommissioner: isCommissioner,
        ),
      ),
    ),
  );
}

DraftSnapshot _lobby({required List<Map<String, Object>> members}) {
  return _snapshot(status: 'lobby', members: members);
}

DraftSnapshot _closed({required List<Map<String, Object>> members}) {
  return _snapshot(status: 'closed', members: members);
}

DraftSnapshot _snapshot({
  required String status,
  required List<Map<String, Object>> members,
  String? currentNominatorUserId,
}) {
  return DraftSnapshot.fromJson({
    'draftId': 7,
    'leagueId': 3,
    'status': status,
    'members': members,
    'currentNominatorUserId': ?currentNominatorUserId,
  });
}

class _FakeDraftHubClient implements DraftHubClient {
  final updatesController = StreamController<DraftSnapshot>.broadcast();
  int? connectedLeagueId;
  var openCalls = 0;
  var closeCalls = 0;
  var startCalls = 0;
  var failClose = false;

  @override
  Stream<DraftSnapshot> get updates => updatesController.stream;

  void emit(DraftSnapshot snapshot) => updatesController.add(snapshot);

  @override
  Future<void> connect({required int leagueId}) async {
    connectedLeagueId = leagueId;
  }

  @override
  Future<void> open() async {
    openCalls++;
  }

  @override
  Future<void> close() async {
    closeCalls++;
    if (failClose) throw Exception('NotCommissioner');
  }

  @override
  Future<void> start() async {
    startCalls++;
  }

  @override
  Future<void> disconnect() async {}
}
