part of 'bid.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Bid _$BidFromJson(Map<String, dynamic> json) => Bid(
  id: (json['id'] as num?)?.toInt(),
  playerId: json['playerId'] as String,
  userId: (json['userId'] as num).toInt(),
  duration: (json['duration'] as num?)?.toInt(),
  salary: (json['salary'] as num?)?.toInt(),
  signingBonus: (json['signingBonus'] as num?)?.toInt(),
  contractRating: (json['contractRating'] as num?)?.toInt(),
);

Map<String, dynamic> _$BidToJson(Bid instance) => <String, dynamic>{
  'id': instance.id,
  'playerId': instance.playerId,
  'userId': instance.userId,
  'duration': instance.duration,
  'salary': instance.salary,
  'signingBonus': instance.signingBonus,
  'contractRating': instance.contractRating,
};