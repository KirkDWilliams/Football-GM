import 'package:dio/dio.dart';
import 'package:football_gm_app/models/player.dart';
import 'package:football_gm_app/models/bid.dart';

abstract class DraftApi 
{
  Future<List<Player>> getAvailablePlayers(int leagueId);
  Future<void> startAuction(int leagueId, String playerId);
  Future<List<Bid>> getAuctionBids(int leagueId, String playerId);
  Future<void> placeBid(int leagueId, int userId, String playerId, num duration, num salary, num? signingBonus);
  Future<void> passAuction(int leagueId, int userId);
  Future<void> processAuction(int leagueId, int userId, List<Bid> winningBids);
}

var getAvailablePlayersUri = (int leagueId) => '/api/league/$leagueId/players/available';
var startAuctionUri = (int leagueId, String playerId) => '/api/league/$leagueId/auction/$playerId/start';
var getAuctionBidsUri = (int leagueId, String playerId) => '/api/league/$leagueId/auction/$playerId/bids';
var placeBidUri = (int leagueId, String playerId) => '/api/league/$leagueId/auction/$playerId/bid';
var passAuctionUri = (int leagueId) => '/api/league/$leagueId/auction/pass';
var processAuctionUri = (int leagueId) => '/api/league/$leagueId/auction/process';

class HttpDraftApi implements DraftApi {
  HttpDraftApi({required this._dio});

  final Dio _dio;

  @override
  Future<List<Player>> getAvailablePlayers(int leagueId) async 
  {
    final response = await _dio.get<List<dynamic>>(
      getAvailablePlayersUri(leagueId)
    );

    if (response.statusCode == 200 && response.data != null) {
      return response.data!
          .map((e) => Player.fromJson(Map<String, dynamic>.from(e as Map)))
          .toList();
    }
    throw Exception('Failed to load players: ${response.statusCode}');
  }

  @override
  Future<void> startAuction(int leagueId, String playerId) async 
  {
    final response = await _dio.post(
      startAuctionUri(leagueId, playerId)
    );
    
    if (response.statusCode != 200) 
    {
      throw Exception('Failed to start auction: ${response.statusCode}');
    }
  }

  @override
  Future<List<Bid>> getAuctionBids(int leagueId, String playerId) async 
  {
    final response = await _dio.get<List<dynamic>>(
      getAuctionBidsUri(leagueId, playerId)
    );

    if (response.statusCode == 200 && response.data != null) {
      return response.data!
          .map((e) => Bid.fromJson(Map<String, dynamic>.from(e as Map)))
          .toList();
    }
    throw Exception('Failed to load bids: ${response.statusCode}');
  }

  @override
  Future<void> placeBid(int leagueId, int userId, String playerId, num duration, num salary, num? signingBonus) async 
  {
    final response = await _dio.post(
      placeBidUri(leagueId, playerId),
      data: 
      {
        'userId': userId,
        'duration': duration, 
        'salary': salary, 
        'signingBonus': signingBonus
      },
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to place bid: ${response.statusCode}');
    }
  }

  @override
  Future<void> passAuction(int leagueId, int userId) async 
  {
    final response = await _dio.post(
      passAuctionUri(leagueId),
      data: 
      {
        'userId': userId
      },
    );

    if (response.statusCode != 200) {
      throw Exception('Failed to pass auction: ${response.statusCode}');
    }
  }

  @override
    Future<void> processAuction(int leagueId, int userId, List<Bid> bids) async 
    {
      final response = await _dio.post(
        processAuctionUri(leagueId),
        data: 
        {
          'userId': userId,
          'winningBids': 
          [
            for (final bid in bids) 
            {
              bid.toJson()
            }
          ]
        },
      );

      if (response.statusCode != 200) {
        throw Exception('Failed to process auction: ${response.statusCode}');
      }
    }
}