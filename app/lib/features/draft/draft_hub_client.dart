import 'dart:async';

import 'package:football_gm_app/features/draft/draft_snapshot.dart';
import 'package:signalr_netcore/signalr_client.dart';

/// SignalR pipe for one draft room. Commands go in; [updates] is `DraftUpdated`.
abstract class DraftHubClient {
  Stream<DraftSnapshot> get updates;

  /// Starts the connection and joins [leagueId]. Joins again after reconnect.
  Future<void> connect({required int leagueId});

  Future<void> open();

  Future<void> close();

  Future<void> start();

  Future<void> disconnect();
}

class SignalRDraftHubClient implements DraftHubClient {
  SignalRDraftHubClient({required this.hubUrl, required this.accessToken});

  final String hubUrl;
  final Future<String?> Function() accessToken;
  final _updates = StreamController<DraftSnapshot>.broadcast();

  HubConnection? _connection;
  int? _leagueId;

  @override
  Stream<DraftSnapshot> get updates => _updates.stream;

  @override
  Future<void> connect({required int leagueId}) async {
    await disconnect();
    _leagueId = leagueId;

    final connection = HubConnectionBuilder()
        .withUrl(
          hubUrl,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => (await accessToken()) ?? '',
            requestTimeout: 15000,
          ),
        )
        .withAutomaticReconnect()
        .build();

    connection.on('DraftUpdated', _onDraftUpdated);
    connection.onreconnected(({connectionId}) {
      final id = _leagueId;
      if (id == null) return;
      unawaited(_join(connection, id));
    });

    await connection.start();
    _connection = connection;
    try {
      await _join(connection, leagueId);
    } on Object {
      await disconnect();
      rethrow;
    }
  }

  @override
  Future<void> open() => _invoke('Open');

  @override
  Future<void> close() => _invoke('Close');

  @override
  Future<void> start() => _invoke('Start');

  @override
  Future<void> disconnect() async {
    final connection = _connection;
    _connection = null;
    if (connection == null) return;
    connection.off('DraftUpdated');
    await connection.stop();
  }

  Future<void> _invoke(String method) async {
    final connection = _connection;
    final leagueId = _leagueId;
    if (connection == null || leagueId == null) {
      throw StateError('Not connected to the draft hub');
    }
    await connection.invoke(method, args: <Object>[leagueId]);
  }

  Future<void> _join(HubConnection connection, int leagueId) =>
      connection.invoke('Join', args: <Object>[leagueId]);

  void _onDraftUpdated(List<Object?>? arguments) {
    if (arguments == null || arguments.isEmpty) return;
    final value = arguments.first;
    if (value is! Map) return;
    _updates.add(DraftSnapshot.fromJson(Map<String, dynamic>.from(value)));
  }
}
