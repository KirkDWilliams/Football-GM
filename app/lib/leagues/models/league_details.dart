import 'package:football_gm_app/leagues/models/league_summary.dart';

/// Full Settings for a League the User belongs to, plus their role.
class LeagueDetails {
  const LeagueDetails({
    required this.leagueId,
    required this.name,
    required this.joinCode,
    required this.weeklyCap,
    required this.role,
    required this.positions,
    required this.rules,
  });

  final int leagueId;
  final String name;
  final String joinCode;
  final num weeklyCap;
  final LeagueRole role;
  final List<Position> positions;
  final List<Rule> rules;

  factory LeagueDetails.fromJson(Map<String, dynamic> json) {
    return LeagueDetails(
      leagueId: json['leagueId'] as int,
      name: json['name'] as String,
      joinCode: json['joinCode'] as String,
      weeklyCap: json['weeklyCapSpace'] as num,
      role: LeagueRole.parse(json['role'] as String),
      positions: [
        for (final position in json['positions'] as List<dynamic>)
          Position.parse(position as Object),
      ],
      rules: [
        for (final rule in json['rules'] as List<dynamic>)
          Rule.fromJson(Map<String, dynamic>.from(rule as Map)),
      ],
    );
  }
}

enum Position {
  quarterback,
  runningBack,
  wideReceiver,
  tightEnd,
  kicker,
  punter;

  String get label => switch (this) {
    quarterback => 'Quarterback',
    runningBack => 'Running Back',
    wideReceiver => 'Wide Receiver',
    tightEnd => 'Tight End',
    kicker => 'Kicker',
    punter => 'Punter',
  };

  static Position parse(Object value) {
    if (value is int) {
      if (value < 0 || value >= Position.values.length) {
        throw FormatException('Unknown position: $value');
      }
      return Position.values[value];
    }
    if (value is String) {
      return switch (value) {
        'quarterback' => quarterback,
        'runningBack' => runningBack,
        'wideReceiver' => wideReceiver,
        'tightEnd' => tightEnd,
        'kicker' => kicker,
        'punter' => punter,
        _ => throw FormatException('Unknown position: $value'),
      };
    }
    throw FormatException('Unknown position: $value');
  }
}

enum StatType {
  passAttempts,
  passCompletions,
  passingYards,
  passingTouchdowns,
  rushingAttempts,
  rushingYards,
  rushingFirstDowns,
  rushingTouchdowns,
  receptions,
  receivingYards,
  receivingTouchdowns,
  interceptions,
  fumbles,
  sacks,
  fieldGoalsMade,
  fieldGoalsMissed,
  extraPointsMade,
  extraPointsAttempted,
  passingTwoPointConversions,
  rushingTwoPointConversions,
  receivingTwoPointConversions,
  returnedTouchdowns;

  String get label => switch (this) {
    passAttempts => 'Pass attempts',
    passCompletions => 'Pass completions',
    passingYards => 'Passing yards',
    passingTouchdowns => 'Passing touchdowns',
    rushingAttempts => 'Rushing attempts',
    rushingYards => 'Rushing yards',
    rushingFirstDowns => 'Rushing first downs',
    rushingTouchdowns => 'Rushing touchdowns',
    receptions => 'Receptions',
    receivingYards => 'Receiving yards',
    receivingTouchdowns => 'Receiving touchdowns',
    interceptions => 'Interceptions',
    fumbles => 'Fumbles',
    sacks => 'Sacks',
    fieldGoalsMade => 'Field goals made',
    fieldGoalsMissed => 'Field goals missed',
    extraPointsMade => 'Extra points made',
    extraPointsAttempted => 'Extra points attempted',
    passingTwoPointConversions => 'Passing two-point conversions',
    rushingTwoPointConversions => 'Rushing two-point conversions',
    receivingTwoPointConversions => 'Receiving two-point conversions',
    returnedTouchdowns => 'Returned touchdowns',
  };

  num get defaultWeight => switch (this) {
    passAttempts => 0,
    passCompletions => 0,
    passingYards => 0.04,
    passingTouchdowns => 4,
    rushingAttempts => 0,
    rushingYards => 0.1,
    rushingFirstDowns => 0,
    rushingTouchdowns => 6,
    receptions => 0,
    receivingYards => 0.1,
    receivingTouchdowns => 6,
    interceptions => -2,
    fumbles => -2,
    sacks => 0,
    fieldGoalsMade => 3,
    fieldGoalsMissed => 0,
    extraPointsMade => 1,
    extraPointsAttempted => 0,
    passingTwoPointConversions => 2,
    rushingTwoPointConversions => 2,
    receivingTwoPointConversions => 2,
    returnedTouchdowns => 6,
  };

  ScoringGroup get group => switch (this) {
    passAttempts ||
    passCompletions ||
    passingYards ||
    passingTouchdowns ||
    passingTwoPointConversions => ScoringGroup.passing,
    rushingAttempts ||
    rushingYards ||
    rushingFirstDowns ||
    rushingTouchdowns ||
    rushingTwoPointConversions => ScoringGroup.rushing,
    receptions ||
    receivingYards ||
    receivingTouchdowns ||
    receivingTwoPointConversions => ScoringGroup.receiving,
    interceptions ||
    fumbles ||
    sacks ||
    returnedTouchdowns => ScoringGroup.turnovers,
    fieldGoalsMade ||
    fieldGoalsMissed ||
    extraPointsMade ||
    extraPointsAttempted => ScoringGroup.kicking,
  };

  static StatType parse(Object value) {
    if (value is int) {
      if (value < 0 || value >= StatType.values.length) {
        throw FormatException('Unknown stat: $value');
      }
      return StatType.values[value];
    }
    if (value is String) {
      return switch (value) {
        'passAttempts' => passAttempts,
        'passCompletions' => passCompletions,
        'passingYards' => passingYards,
        'passingTouchdowns' => passingTouchdowns,
        'rushingAttempts' => rushingAttempts,
        'rushingYards' => rushingYards,
        'rushingFirstDowns' => rushingFirstDowns,
        'rushingTouchdowns' => rushingTouchdowns,
        'receptions' => receptions,
        'receivingYards' => receivingYards,
        'receivingTouchdowns' => receivingTouchdowns,
        'interceptions' => interceptions,
        'fumbles' => fumbles,
        'sacks' => sacks,
        'fieldGoalsMade' => fieldGoalsMade,
        'fieldGoalsMissed' => fieldGoalsMissed,
        'extraPointsMade' => extraPointsMade,
        'extraPointsAttempted' => extraPointsAttempted,
        'passingTwoPointConversions' => passingTwoPointConversions,
        'rushingTwoPointConversions' => rushingTwoPointConversions,
        'receivingTwoPointConversions' => receivingTwoPointConversions,
        'returnedTouchdowns' => returnedTouchdowns,
        _ => throw FormatException('Unknown stat: $value'),
      };
    }
    throw FormatException('Unknown stat: $value');
  }
}

enum ScoringGroup {
  passing,
  rushing,
  receiving,
  turnovers,
  kicking;

  String get label => switch (this) {
    passing => 'Passing',
    rushing => 'Rushing',
    receiving => 'Receiving',
    turnovers => 'Turnovers',
    kicking => 'Kicking',
  };
}

sealed class Rule {
  const Rule({required this.stat});

  final StatType stat;

  factory Rule.fromJson(Map<String, dynamic> json) {
    final type =
        json[r'$type'] as String? ?? _typeFromRuleType(json['ruleType']);
    final stat = StatType.parse(json['stat'] as Object);
    return switch (type) {
      'scoringWeight' => ScoringWeightRule(
        stat: stat,
        weight: json['weight'] as num,
      ),
      'bonus' => BonusRule(
        stat: stat,
        threshold: json['threshold'] as num,
        points: json['points'] as num,
      ),
      _ => throw FormatException('Unknown rule type: $type'),
    };
  }

  static String _typeFromRuleType(Object? value) {
    if (value == 0 || value == 'scoringWeight') return 'scoringWeight';
    if (value == 1 || value == 'bonus') return 'bonus';
    throw FormatException('Unknown rule type: $value');
  }
}

final class ScoringWeightRule extends Rule {
  const ScoringWeightRule({required super.stat, required this.weight});

  final num weight;

  Map<String, dynamic> toJson() => {
    r'$type': 'scoringWeight',
    'stat': stat.index,
    'weight': weight,
  };

  static List<ScoringWeightRule> defaults() => [
    for (final stat in StatType.values)
      ScoringWeightRule(stat: stat, weight: stat.defaultWeight),
  ];

  static bool usesDefaultScoringWeights(Iterable<ScoringWeightRule> rules) {
    final expected = {for (final rule in defaults()) rule.stat: rule.weight};
    final actual = {for (final rule in rules) rule.stat: rule.weight};
    if (expected.length != actual.length) return false;
    for (final entry in expected.entries) {
      if (actual[entry.key] != entry.value) return false;
    }
    return true;
  }
}

final class BonusRule extends Rule {
  const BonusRule({
    required super.stat,
    required this.threshold,
    required this.points,
  });

  final num threshold;
  final num points;
}
