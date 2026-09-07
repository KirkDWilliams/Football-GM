import 'package:json_annotation/json_annotation.dart';

part 'bid.g.dart';

@JsonSerializable()
class Bid {
  final int? id;
  final String playerId;
  final int userId;
  final int? duration;
  final int? salary;
  final int? signingBonus;
  final int? contractRating;

  Bid(
    {
      this.id, 
      required this.playerId,
      this.userId, 
      required this.duration,
      this.salary,
      this.signingBonus,
      this.contractRating,
    }
  );

  factory Bid.fromJson(Map<String, dynamic> json) => _$BidFromJson(json);
  Map<String, dynamic> toJson() => _$BidToJson(this);
}
