import 'package:json_annotation/json_annotation.dart';

part 'player.g.dart';

@JsonSerializable()
class Player {
  final String id;
  final String name;
  final int? teamId;
  final String position;
  final String nflTeam;
  final int? minimumContractRating;

  Player(
    {
      required this.id, 
      required this.name,
      this.teamId,
      required this.position,
      required this.nflTeam,
      this.minimumContractRating
    }
  );

  factory Player.fromJson(Map<String, dynamic> json) => _$PlayerFromJson(json);
  Map<String, dynamic> toJson() => _$PlayerToJson(this);
}
