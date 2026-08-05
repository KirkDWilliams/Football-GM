// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'game.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Game _$GameFromJson(Map<String, dynamic> json) => Game(
  id: (json['id'] as num?)?.toInt(),
  homeTeamId: (json['homeTeamId'] as num).toInt(),
  awayTeamId: (json['awayTeamId'] as num).toInt(),
  date: DateTime.parse(json['date'] as String),
  homeScore: (json['homeScore'] as num?)?.toInt(),
  awayScore: (json['awayScore'] as num?)?.toInt(),
);

Map<String, dynamic> _$GameToJson(Game instance) => <String, dynamic>{
  'id': instance.id,
  'homeTeamId': instance.homeTeamId,
  'awayTeamId': instance.awayTeamId,
  'date': instance.date.toIso8601String(),
  'homeScore': instance.homeScore,
  'awayScore': instance.awayScore,
};
