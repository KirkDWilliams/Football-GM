import 'dart:async';

import 'package:flutter/material.dart';
import 'package:football_gm_app/features/draft/draft_hub_client.dart';
import 'package:football_gm_app/features/draft/draft_snapshot.dart';
import 'package:football_gm_app/ui/ui.dart';

class DraftRoomScreen extends StatefulWidget {
  const DraftRoomScreen({
    super.key,
    required this.client,
    required this.leagueId,
    required this.isCommissioner,
  });

  final DraftHubClient client;
  final int leagueId;
  final bool isCommissioner;

  @override
  State<DraftRoomScreen> createState() => _DraftRoomScreenState();
}

class _DraftRoomScreenState extends State<DraftRoomScreen> {
  StreamSubscription<DraftSnapshot>? _subscription;
  DraftSnapshot? _snapshot;
  bool _connecting = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _subscription = widget.client.updates.listen((snapshot) {
      if (!mounted) return;
      setState(() {
        _snapshot = snapshot;
        _connecting = false;
        _error = null;
      });
    });
    _connect();
  }

  Future<void> _connect() async {
    try {
      await widget.client.connect(leagueId: widget.leagueId);
      if (!mounted) return;
      setState(() {
        _connecting = false;
        _error = null;
      });
    } on Object catch (error) {
      if (!mounted) return;
      setState(() {
        _connecting = false;
        _error = 'Could not connect: $error';
      });
    }
  }

  @override
  void dispose() {
    _subscription?.cancel();
    unawaited(widget.client.disconnect());
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final snapshot = _snapshot;
    return ArcadePage(
      title: 'Draft',
      maxWidth: 480,
      body: _connecting
          ? const Center(child: CircularProgressIndicator())
          : ListView(
              children: [
                if (_error != null) StatusBanner(text: _error!),
                Text(
                  snapshot == null ? 'No open draft' : snapshot.status.label,
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineSmall,
                ),
                if (snapshot != null) ...[
                  const SizedBox(height: 16),
                  for (final member in snapshot.members)
                    ListTile(
                      contentPadding: EdgeInsets.zero,
                      title: Text(member.displayName),
                    ),
                ],
                if (_nominatorLine(snapshot) case final line?) ...[
                  const SizedBox(height: 16),
                  Text(line, textAlign: TextAlign.center),
                ],
                if (_showOpen(snapshot)) ...[
                  const SizedBox(height: 24),
                  FilledButton(
                    onPressed: () => _run(widget.client.open),
                    child: const Text('Open'),
                  ),
                ],
                if (_inLobby(snapshot)) ...[
                  const SizedBox(height: 24),
                  FilledButton(
                    onPressed: () => _run(widget.client.close),
                    child: const Text('Close'),
                  ),
                  const SizedBox(height: 12),
                  FilledButton(
                    onPressed: () => _run(widget.client.start),
                    child: const Text('Start'),
                  ),
                ],
              ],
            ),
    );
  }

  bool _showOpen(DraftSnapshot? snapshot) {
    if (!widget.isCommissioner) return false;
    return snapshot == null || snapshot.status == DraftStatus.closed;
  }

  bool _inLobby(DraftSnapshot? snapshot) =>
      widget.isCommissioner && snapshot?.status == DraftStatus.lobby;

  String? _nominatorLine(DraftSnapshot? snapshot) {
    if (snapshot == null || snapshot.status != DraftStatus.live) return null;
    final id = snapshot.currentNominatorUserId;
    if (id == null || id.isEmpty) return null;
    for (final member in snapshot.members) {
      if (member.userId == id) {
        return 'Up to nominate: ${member.displayName}';
      }
    }
    return 'Up to nominate: $id';
  }

  Future<void> _run(Future<void> Function() command) async {
    try {
      await command();
    } on Object catch (error) {
      if (!mounted) return;
      setState(() => _error = _draftCommandError(error));
    }
  }
}

String _draftCommandError(Object error) {
  final raw = error.toString();
  if (raw.contains('NotCommissioner')) {
    return 'Only the commissioner can do that';
  }
  if (raw.contains('ActiveDraft')) return 'A draft is already open';
  if (raw.contains('DraftInactive') || raw.contains('NotInLobby')) {
    return 'The draft is not in the lobby';
  }
  if (raw.contains('NoDraftFound') || raw.contains('LobbyNotStarted')) {
    return 'No open draft';
  }
  if (raw.contains('UserNotInLeague')) return 'You are not in this league';
  if (raw.contains('LeagueDraftCompleted')) {
    return "This league's draft is finished";
  }
  return raw;
}
