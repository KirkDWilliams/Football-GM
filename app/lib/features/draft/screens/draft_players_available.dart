import 'package:flutter/material.dart';
import 'package:football_gm_app/features/draft/draft_api.dart';
import 'package:football_gm_app/features/leagues/league_api.dart';
import 'package:football_gm_app/models/player.dart';
import 'package:football_gm_app/leagues/screens/auction_screen.dart';
import 'package:football_gm_app/ui/ui.dart';
import 'package:provider/provider.dart';

class PlayersAvailableScreen extends StatefulWidget {
  const PlayersAvailableScreen({
    super.key,
    required this.leagueId,
    required this.draftApi,
    required this.leagueName,
  });

  final int leagueId;
  final DraftApi draftApi;
  final String leagueName;

  @override
  State<PlayersAvailableScreen> createState() => _PlayersAvailableScreenState();
}

class _PlayersAvailableScreenState extends State<PlayersAvailableScreen> {
  late Future<List<Player>> _playersFuture;

  @override
  void initState() {
    super.initState();
    _playersFuture = widget.draftApi.getAvailablePlayers(widget.leagueId);
  }

  @override
  Widget build(BuildContext context) {
    return ArcadePage(
      title: '${widget.leagueName} - Draft',
      maxWidth: 800,
      body: FutureBuilder<List<Player>>(
        future: _playersFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Text('Error: ${snapshot.error}'),
                  const SizedBox(height: 16),
                  FilledButton(
                    onPressed: () => setState(() {
                      _playersFuture = widget.draftApi.getAvailablePlayers(
                        widget.leagueId,
                      );
                    }),
                    child: const Text('Retry'),
                  ),
                ],
              ),
            );
          }

          final players = snapshot.data ?? [];
          if (players.isEmpty) {
            return const Center(child: Text('No players available'));
          }

          return ListView.builder(
            itemCount: players.length,
            itemBuilder: (context, index) {
              final player = players[index];
              return Card(
                child: ListTile(
                  leading: Container(
                    width: 40,
                    height: 40,
                    decoration: BoxDecoration(
                      color: Colors.blue[200],
                      shape: BoxShape.circle,
                    ),
                    child: Center(
                      child: Text(
                        player.position.substring(0, 1),
                        style: const TextStyle(
                          fontWeight: FontWeight.bold,
                          color: Colors.black,
                        ),
                      ),
                    ),
                  ),
                  title: Text(player.name),
                  subtitle: Text('${player.} • ${player.position}'),
///                  trailing: Text('\$${ here we should indicate how expensive the player is likely to be to start the draft... might give some indication that the player is expected to be a star or a fremium)}'),
                  onTap: () => _navigateToAuction(context, player),
                ),
              );
            },
          );
        },
      ),
    );
  }

  Future<void> _navigateToAuction(BuildContext context, Player player) {
    return Navigator.of(context).push(
      MaterialPageRoute<void>(
        builder: (_) => AuctionScreen(
          leagueId: widget.leagueId,
          player: player,
          leagueApi: context.read<LeagueApi>(),
        ),
      ),
    );
  }
}