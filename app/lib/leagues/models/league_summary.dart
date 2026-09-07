/// Slim list row for a League the User belongs to.
class LeagueSummary {
  const LeagueSummary({
    required this.leagueId,
    required this.name,
    required this.joinCode,
    required this.role,
    required this.scoring,
  });

  final int leagueId;
  final String name;
  final String joinCode;
  final LeagueRole role;
  final ScoringKind scoring;

  factory LeagueSummary.fromJson(Map<String, dynamic> json) {
    return LeagueSummary(
      leagueId: json['leagueId'] as int,
      name: json['name'] as String,
      joinCode: json['joinCode'] as String,
      role: LeagueRole.parse(json['role'] as String),
      scoring: ScoringKind.parse(json['scoring'] as String),
    );
  }
}

enum LeagueRole {
  member,
  admin,
  commissioner;

  String get label => switch (this) {
        member => 'Member',
        admin => 'Admin',
        commissioner => 'Commissioner',
      };

  static LeagueRole parse(String value) => switch (value) {
        'member' => member,
        'admin' => admin,
        'commissioner' => commissioner,
        _ => throw FormatException('Unknown role: $value'),
      };
}

enum ScoringKind {
  standard,
  custom;

  String get label => switch (this) {
        standard => 'Standard scoring',
        custom => 'Custom scoring',
      };

  static ScoringKind parse(String value) => switch (value) {
        'standard' => standard,
        'custom' => custom,
        _ => throw FormatException('Unknown scoring: $value'),
      };
}
