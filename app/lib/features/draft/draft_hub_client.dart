import 'dart:async';

import 'package:signalr_netcore/signalr_client.dart';

// PLAYGROUND: counter client. Keep SignalR connect/Join/disconnect; drop increment/decrement/CounterChanged.
abstract class DraftHubClient {
  Stream<int> get counter;

  Future<void> connect({required String draftId});

  Future<void> increment();

  Future<void> decrement();

  Future<void> disconnect();
}

class SignalRDraftHubClient implements DraftHubClient {
  SignalRDraftHubClient({
    required this.hubUrl,
    required this.accessToken,
  });

  final String hubUrl;
  final Future<String?> Function() accessToken;
  final _counter = StreamController<int>.broadcast();

  HubConnection? _connection;
  String? _draftId;

  @override
  Stream<int> get counter => _counter.stream;

  @override
  Future<void> connect({required String draftId}) async {
    await disconnect();
    _draftId = draftId;

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

    connection.on('CounterChanged', _onCounterChanged);
    connection.onreconnected(({connectionId}) {
      final id = _draftId;
      if (id == null) return;
      unawaited(connection.invoke('Join', args: <Object>[id]));
    });

    await connection.start();
    await connection.invoke('Join', args: <Object>[draftId]);
    _connection = connection;
  }

  @override
  Future<void> increment() {
    return _invoke('Increment');
  }

  @override
  Future<void> decrement() {
    return _invoke('Decrement');
  }

  @override
  Future<void> disconnect() async {
    final connection = _connection;
    _connection = null;
    if (connection == null) return;
    connection.off('CounterChanged');
    await connection.stop();
  }

  Future<void> _invoke(String method) async {
    final connection = _connection;
    final draftId = _draftId;
    if (connection == null || draftId == null) {
      throw StateError('Not connected to the draft hub');
    }
    await connection.invoke(method, args: <Object>[draftId]);
  }

  void _onCounterChanged(List<Object?>? arguments) {
    if (arguments == null || arguments.isEmpty) return;
    final value = arguments.first;
    if (value is num) _counter.add(value.toInt());
  }
}
