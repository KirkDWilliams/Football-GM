/// Picture of a draft room. Replaced wholesale on each `DraftUpdated`.
class DraftSnapshot {
  const DraftSnapshot({
    required this.draftId,
    required this.leagueId,
    required this.status,
    required this.members,
    this.currentNominatorUserId,
  });

  final int draftId;
  final int leagueId;
  final DraftStatus status;
  final List<DraftMember> members;
  final String? currentNominatorUserId;

  factory DraftSnapshot.fromJson(Map<String, dynamic> json) {
    final members = json['members'];
    if (members is! List) {
      throw const FormatException('Draft snapshot is missing members');
    }
    return DraftSnapshot(
      draftId: _asInt(json['draftId']),
      leagueId: _asInt(json['leagueId']),
      status: DraftStatus.parse(json['status'] as String),
      members: [
        for (final member in members)
          DraftMember.fromJson(Map<String, dynamic>.from(member as Map)),
      ],
      currentNominatorUserId: json['currentNominatorUserId'] as String?,
    );
  }
}

class DraftMember {
  const DraftMember({required this.userId, required this.displayName});

  final String userId;
  final String displayName;

  factory DraftMember.fromJson(Map<String, dynamic> json) {
    return DraftMember(
      userId: json['userId'] as String,
      displayName: json['displayName'] as String,
    );
  }
}

enum DraftStatus {
  lobby,
  live,
  complete,
  closed;

  String get label => switch (this) {
    lobby => 'Lobby',
    live => 'Live',
    complete => 'Complete',
    closed => 'Closed',
  };

  static DraftStatus parse(String value) => switch (value) {
    'lobby' => lobby,
    'live' => live,
    'complete' => complete,
    'closed' => closed,
    _ => throw FormatException('Unknown draft status: $value'),
  };
}

int _asInt(Object? value) {
  if (value is int) return value;
  if (value is num) return value.toInt();
  throw FormatException('Expected an int, got $value');
}
