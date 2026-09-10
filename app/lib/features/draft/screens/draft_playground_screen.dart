import 'dart:async';

import 'package:flutter/material.dart';
import 'package:football_gm_app/features/draft/draft_hub_client.dart';
import 'package:football_gm_app/ui/ui.dart';

// PLAYGROUND: delete this file.
class DraftPlaygroundScreen extends StatefulWidget {
  const DraftPlaygroundScreen({
    super.key,
    required this.client,
    this.draftId = 'playground',
  });

  final DraftHubClient client;
  final String draftId;

  @override
  State<DraftPlaygroundScreen> createState() => _DraftPlaygroundScreenState();
}

class _DraftPlaygroundScreenState extends State<DraftPlaygroundScreen> {
  StreamSubscription<int>? _subscription;
  int? _value;
  bool _connecting = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _subscription = widget.client.counter.listen((value) {
      if (!mounted) return;
      setState(() {
        _value = value;
        _connecting = false;
        _error = null;
      });
    });
    _connect();
  }

  Future<void> _connect() async {
    try {
      await widget.client.connect(draftId: widget.draftId);
    } on Object catch (e) {
      if (!mounted) return;
      setState(() {
        _connecting = false;
        _error = 'Could not connect: $e';
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
    return ArcadePage(
      title: 'SignalR playground',
      maxWidth: 480,
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (_error != null) StatusBanner(text: _error!),
          const Spacer(),
          if (_connecting)
            const Center(child: CircularProgressIndicator())
          else
            Text(
              '${_value ?? '—'}',
              textAlign: TextAlign.center,
              style: ArcadeFonts.body(size: 72, color: ArcadeColors.gold),
            ),
          const SizedBox(height: 24),
          Row(
            children: [
              Expanded(
                child: FilledButton(
                  onPressed: _connecting ? null : widget.client.decrement,
                  child: const Text('−'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: FilledButton(
                  onPressed: _connecting ? null : widget.client.increment,
                  child: const Text('+'),
                ),
              ),
            ],
          ),
          const Spacer(),
        ],
      ),
    );
  }
}
