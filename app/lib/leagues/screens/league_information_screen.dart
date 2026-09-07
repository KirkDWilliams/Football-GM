import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/models/league_details.dart';
import 'package:football_gm_app/ui/widgets/arcade_accordion.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';

class LeagueInformationScreen extends StatefulWidget {
  const LeagueInformationScreen({
    super.key,
    required this.leagueId,
    required this.leagueApi,
  });

  final int leagueId;
  final LeagueApi leagueApi;

  @override
  State<LeagueInformationScreen> createState() =>
      _LeagueInformationScreenState();
}

class _LeagueInformationScreenState extends State<LeagueInformationScreen> {
  LeagueDetails? _league;
  bool _loading = true;
  bool _failed = false;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final league = await widget.leagueApi.getLeague(widget.leagueId);
      if (!mounted) return;
      setState(() {
        _league = league;
        _loading = false;
      });
    } on Object {
      if (!mounted) return;
      setState(() {
        _failed = true;
        _loading = false;
      });
    }
  }

  Future<void> _copyJoinCode(String joinCode) async {
    await Clipboard.setData(ClipboardData(text: joinCode));
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(const SnackBar(content: Text('Join code copied')));
  }

  @override
  Widget build(BuildContext context) {
    return ArcadePage(
      title: _league?.name ?? 'League',
      maxWidth: 640,
      body: _body(),
    );
  }

  Widget _body() {
    if (_loading) return const Center(child: CircularProgressIndicator());
    final league = _league;
    if (_failed || league == null) {
      return const Center(child: Text('League not found'));
    }
    return _LeagueBody(
      league: league,
      onCopyJoinCode: () => _copyJoinCode(league.joinCode),
    );
  }
}

class _LeagueBody extends StatelessWidget {
  const _LeagueBody({required this.league, required this.onCopyJoinCode});

  final LeagueDetails league;
  final VoidCallback onCopyJoinCode;

  @override
  Widget build(BuildContext context) {
    final scoringWeights = league.rules.whereType<ScoringWeightRule>().toList();
    final bonuses = league.rules.whereType<BonusRule>().toList();

    return ListView(
      children: [
        ListTile(
          contentPadding: EdgeInsets.zero,
          dense: true,
          title: const Text('Name'),
          subtitle: Text(league.name),
        ),
        ListTile(
          contentPadding: EdgeInsets.zero,
          dense: true,
          title: const Text('Join code'),
          subtitle: Text(league.joinCode),
          trailing: const Icon(Icons.copy),
          onTap: onCopyJoinCode,
        ),
        ListTile(
          contentPadding: EdgeInsets.zero,
          dense: true,
          title: const Text('Role'),
          subtitle: Text(league.role.label),
        ),
        ListTile(
          contentPadding: EdgeInsets.zero,
          dense: true,
          title: const Text('Weekly cap'),
          subtitle: Text(formatLeagueNumber(league.weeklyCap)),
        ),
        const SizedBox(height: 8),
        Text(
          'Eligible positions',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 8),
        Wrap(
          spacing: 8,
          runSpacing: 8,
          children: [
            for (final position in league.positions)
              Chip(label: Text(position.label)),
          ],
        ),
        const SizedBox(height: 16),
        ArcadeAccordion(
          title: 'Scoring weights',
          child: _ScoringWeightGroups(rules: scoringWeights),
        ),
        if (bonuses.isNotEmpty) ...[
          const SizedBox(height: 8),
          ArcadeAccordion(
            title: 'Bonuses',
            child: Column(
              children: [
                for (final bonus in bonuses)
                  ListTile(
                    contentPadding: EdgeInsets.zero,
                    title: Text(bonus.stat.label),
                    trailing: Text(
                      '${formatLeagueNumber(bonus.points)} points at ${formatLeagueNumber(bonus.threshold)}',
                    ),
                  ),
              ],
            ),
          ),
        ],
      ],
    );
  }
}

class _ScoringWeightGroups extends StatelessWidget {
  const _ScoringWeightGroups({required this.rules});

  final List<ScoringWeightRule> rules;

  @override
  Widget build(BuildContext context) {
    return Column(
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
          for (final rule in rules.where((rule) => rule.stat.group == group))
            ListTile(
              contentPadding: EdgeInsets.zero,
              dense: true,
              title: Text(rule.stat.label),
              trailing: Text(formatLeagueNumber(rule.weight)),
            ),
        ],
      ],
    );
  }
}
