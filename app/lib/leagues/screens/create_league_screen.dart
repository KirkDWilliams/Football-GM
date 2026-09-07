import 'package:flutter/material.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/screens/league_information_screen.dart';
import 'package:provider/provider.dart';

class CreateLeagueScreen extends StatefulWidget {
  const CreateLeagueScreen({super.key});

  @override
  State<CreateLeagueScreen> createState() => _CreateLeagueScreenState();
}

class _CreateLeagueScreenState extends State<CreateLeagueScreen> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _weeklyCap = TextEditingController(text: '100');
  final _weightControllers = {
    for (final stat in StatType.values)
      stat: TextEditingController(text: _formatNumber(stat.defaultWeight)),
  };
  bool _busy = false;
  String? _error;

  @override
  void dispose() {
    _name.dispose();
    _weeklyCap.dispose();
    for (final controller in _weightControllers.values) {
      controller.dispose();
    }
    super.dispose();
  }

  Future<void> _submit() async {
    setState(() => _error = null);
    if (!(_formKey.currentState?.validate() ?? false)) return;

    setState(() => _busy = true);
    try {
      final api = context.read<LeagueApi>();
      final scoringWeights = [
        for (final stat in StatType.values)
          ScoringWeightRule(
            stat: stat,
            weight: num.parse(_weightControllers[stat]!.text.trim()),
          ),
      ];
      final leagueId = await api.createLeague(
        name: _name.text.trim(),
        weeklyCap: num.parse(_weeklyCap.text.trim()),
        scoringWeights:
            ScoringWeightRule.usesDefaultScoringWeights(scoringWeights)
            ? null
            : scoringWeights,
      );
      if (!mounted) return;
      Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) =>
              LeagueInformationScreen(leagueId: leagueId, leagueApi: api),
        ),
      );
    } on Object {
      if (!mounted) return;
      setState(() {
        _busy = false;
        _error = 'Could not create League';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Create League')),
      body: Center(
        child: ConstrainedBox(
          constraints: const BoxConstraints(maxWidth: 400),
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: Form(
              key: _formKey,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  if (_error != null)
                    Padding(
                      padding: const EdgeInsets.only(bottom: 16),
                      child: Text(
                        _error!,
                        style: TextStyle(
                          color: Theme.of(context).colorScheme.error,
                        ),
                      ),
                    ),
                  TextFormField(
                    controller: _name,
                    enabled: !_busy,
                    textCapitalization: TextCapitalization.words,
                    decoration: const InputDecoration(
                      labelText: 'Name',
                      border: OutlineInputBorder(),
                    ),
                    validator: (value) {
                      if (value == null || value.trim().isEmpty) {
                        return 'Name is required';
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _weeklyCap,
                    enabled: !_busy,
                    keyboardType: const TextInputType.numberWithOptions(
                      decimal: true,
                      signed: true,
                    ),
                    decoration: const InputDecoration(
                      labelText: 'Weekly cap',
                      border: OutlineInputBorder(),
                    ),
                    validator: (value) {
                      final cap = num.tryParse(value?.trim() ?? '');
                      if (cap == null || cap <= 0) {
                        return 'Weekly cap must be greater than 0';
                      }
                      return null;
                    },
                  ),
                  const SizedBox(height: 16),
                  _ScoringWeightEditor(
                    controllers: _weightControllers,
                    enabled: !_busy,
                  ),
                  const SizedBox(height: 24),
                  FilledButton(
                    onPressed: _busy ? null : _submit,
                    child: _busy
                        ? const SizedBox(
                            height: 20,
                            width: 20,
                            child: CircularProgressIndicator(strokeWidth: 2),
                          )
                        : const Text('Create'),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _ScoringWeightEditor extends StatefulWidget {
  const _ScoringWeightEditor({
    required this.controllers,
    required this.enabled,
  });

  final Map<StatType, TextEditingController> controllers;
  final bool enabled;

  @override
  State<_ScoringWeightEditor> createState() => _ScoringWeightEditorState();
}

class _ScoringWeightEditorState extends State<_ScoringWeightEditor> {
  bool _open = false;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ListTile(
          contentPadding: EdgeInsets.zero,
          title: const Text('Scoring weights'),
          trailing: Icon(_open ? Icons.expand_less : Icons.expand_more),
          onTap: () => setState(() => _open = !_open),
        ),
        if (_open)
          for (final group in ScoringGroup.values) ...[
            Padding(
              padding: const EdgeInsets.only(top: 8, bottom: 4),
              child: Text(
                group.label,
                style: Theme.of(context).textTheme.titleSmall,
              ),
            ),
            for (final stat in StatType.values.where(
              (stat) => stat.group == group,
            )) ...[
              TextFormField(
                controller: widget.controllers[stat],
                enabled: widget.enabled,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                  signed: true,
                ),
                decoration: InputDecoration(
                  labelText: stat.label,
                  border: const OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 12),
            ],
          ],
      ],
    );
  }
}

String _formatNumber(num value) {
  if (value is int) return value.toString();
  if (value == value.roundToDouble()) return value.toInt().toString();
  return value.toString();
}
