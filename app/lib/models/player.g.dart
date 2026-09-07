// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'player.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Player _$PlayerFromJson(Map<String, dynamic> json) => Player(
  id: json['id'] as String,
  name: json['name'] as String,
  teamId: (json['teamId'] as num).toInt(),
  position: json['position'] as String,
  nflTeam: json['nflTeam'] as String,
  minimumContractRating: json['minimumContractRating'] as int?,
);

Map<String, dynamic> _$PlayerToJson(Player instance) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'teamId': instance.teamId,
  'position': instance.position,
  'nflTeam': instance.nflTeam,
  'minimumContractRating': instance.minimumContractRating,
};
