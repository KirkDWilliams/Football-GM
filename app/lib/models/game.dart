import 'package:json_annotation/json_annotation.dart';

part 'game.g.dart';

@JsonSerializable()
class Game {
  final int? id;
  final int homeTeamId;
  final int awayTeamId;
  final DateTime date;
  final int? homeScore;
  final int? awayScore;

  Game({this.id, required this.homeTeamId, required this.awayTeamId, required this.date, this.homeScore, this.awayScore});

  factory Game.fromJson(Map<String, dynamic> json) => _$GameFromJson(json);
  Map<String, dynamic> toJson() => _$GameToJson(this);
}
