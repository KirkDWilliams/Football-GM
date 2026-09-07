import 'package:flutter/material.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/leagues/screens/league_information_screen.dart';
import 'package:football_gm_app/ui/widgets/arcade_accordion.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/arcade_submit_button.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';
import 'package:football_gm_app/ui/widgets/status_banner.dart';
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
      stat: TextEditingController(text: formatLeagueNumber(stat.defaultWeight)),
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
    return ArcadePage(
      title: 'Create League',
      maxWidth: 480,
      body: PixelPanel(
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (_error != null) StatusBanner(text: _error!),
                TextFormField(
                  controller: _name,
                  enabled: !_busy,
                  textCapitalization: TextCapitalization.words,
                  decoration: const InputDecoration(labelText: 'Name'),
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
                  decoration: const InputDecoration(labelText: 'Weekly cap'),
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
                ArcadeSubmitButton(
                  label: 'Create',
                  busy: _busy,
                  onPressed: _submit,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}

class _ScoringWeightEditor extends StatelessWidget {
  const _ScoringWeightEditor({
    required this.controllers,
    required this.enabled,
  });

  final Map<StatType, TextEditingController> controllers;
  final bool enabled;

  @override
  Widget build(BuildContext context) {
    return ArcadeAccordion(
      title: 'Scoring weights',
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          for (final group in ScoringGroup.values) ...[
            Padding(
              padding: const EdgeInsets.only(top: 8, bottom: 4),
              child: Text(
                group.label,
                style: Theme.of(context).textTheme.titleSmall,
              ),
            ),
            for (final stat in group.stats) ...[
              TextFormField(
                controller: controllers[stat],
                enabled: enabled,
                keyboardType: const TextInputType.numberWithOptions(
                  decimal: true,
                  signed: true,
                ),
                decoration: InputDecoration(labelText: stat.label),
              ),
              const SizedBox(height: 12),
            ],
          ],
        ],
      ),
    );
  }
}
