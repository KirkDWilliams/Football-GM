import 'dart:async';

import 'package:flutter/material.dart';
import 'package:football_gm_app/features/leagues/league_api.dart';
import 'package:football_gm_app/features/draft/draft_api.dart';
import 'package:football_gm_app/models/bid.dart';
import 'package:football_gm_app/models/player.dart';
import 'package:football_gm_app/ui/ui.dart';

class AuctionScreen extends StatefulWidget {
  const AuctionScreen(
    {
      super.key,
      required this.leagueId,
      required this.player,
      required this.draftApi,
      required this.leagueApi,
    }
  );

  final int leagueId;
  final Player player;
  final DraftApi draftApi;
  final LeagueApi leagueApi;

  @override
  State<AuctionScreen> createState() => _AuctionScreenState();
}

class _AuctionScreenState extends State<AuctionScreen> {
  late Timer _bidRefreshTimer;
  late Bid _currentBid;
  bool _hasPasssed = false;
  List<Bid> _bids = [];
  String? _error;
  bool _isLoading = false;

  final _bidAmountController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _currentBid = Bid(salary: 0, signingBonus: 0, duration: 3);
    _startAuction();
    _startBidRefresh();
  }

  @override
  void dispose() {
    _bidRefreshTimer.cancel();
    _bidAmountController.dispose();
    super.dispose();
  }

  Future<void> _startAuction() async {
    try {
      await widget.draftApi.startAuction(widget.leagueId, widget.player.id);
      await _refreshBids();
    } on Object catch (e) {
      _showError('Failed to start auction: $e');
    }
  }

  void _startBidRefresh() {
    _bidRefreshTimer = Timer.periodic(const Duration(seconds: 1), (_) {
      _refreshBids();
    });
  }

  Future<void> _refreshBids() async {
    try {
      final bids = await widget.draftApi.getAuctionBids(widget.leagueId, widget.player.id);

      if (!mounted) return;

      setState(() 
      {
        _bids = bids;
        if (bids.isNotEmpty) 
        {
          _currentBid = bids.last;
        }
        _error = null;
      });
    } on Object catch (e) 
    {
      _showError('Failed to load bids: $e');
    }
  }

  Future<void> _placeBid() async {
    final amount = num.tryParse(_bidAmountController.text);
    if (amount == null || amount <= _currentBid) 
    {
      _showError('Your bid is less preferred than the current prevailing bid');
      return;
    }

    setState(() => _isLoading = true);
    try 
    {
      await widget.draftApi.placeBid(widget.leagueId, widget.userId, widget.player.id, amount);
      _bidAmountController.clear();
      await _refreshBids();
    } on Object catch (e) {
      _showError('Failed to place bid: $e');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  Future<void> _pass() async {
    setState(() => _isLoading = true);
    try {
      await widget.draftApi.passAuction(widget.leagueId, widget.player.id);
      setState(() => _hasPasssed = true);
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('You passed on this player')),
      );
    } on Object catch (e) {
      _showError('Failed to pass: $e');
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  void _showError(String message) {
    if (!mounted) return;
    setState(() => _error = message);
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: Colors.red,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ArcadePage(
      title: widget.player.name,
      maxWidth: 640,
      body: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            // Player info card
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.player.name,
                      style: Theme.of(context).textTheme.headlineSmall,
                    ),
                    const SizedBox(height: 8),
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(widget.player.position),
                        Text(widget.player.nflTeam),
                      ],
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),

            // Current bid display
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  children: [
                    Text(
                      'Current Bid',
                      style: Theme.of(context).textTheme.titleMedium,
                    ),
                    const SizedBox(height: 8),
                    Text(
                      '\$${_currentBid.contractRating?.toStringAsFixed(0)}',
                      style: Theme.of(context)
                          .textTheme
                          .headlineLarge
                          ?.copyWith(color: Colors.green),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),

            // Bid input
            if (!_hasPasssed) ...[
              TextField(
                controller: _bidAmountController,
                keyboardType: const TextInputType.numberWithOptions(decimal: true),
                decoration: InputDecoration(
                  labelText: 'Enter bid amount (min: \$${(_currentBid + 1).toStringAsFixed(0)})',
                  border: const OutlineInputBorder(),
                ),
              ),
              const SizedBox(height: 16),

              // Action buttons
              Row(
                children: [
                  Expanded(
                    child: FilledButton(
                      onPressed: _isLoading ? null : _placeBid,
                      child: _isLoading
                          ? const SizedBox(
                              height: 20,
                              width: 20,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            )
                          : const Text('Place Bid'),
                    ),
                  ),
                  const SizedBox(width: 8),
                  FilledButton.tonal(
                    onPressed: _isLoading ? null : _pass,
                    child: const Text('Pass'),
                  ),
                ],
              ),
            ] else
              Center(
                child: Text(
                  'You have passed on this player',
                  style: Theme.of(context).textTheme.bodyLarge,
                ),
              ),
          ],
        ),
      ),
    );
  }
}