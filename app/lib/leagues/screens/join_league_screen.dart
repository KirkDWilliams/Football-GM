import 'package:flutter/material.dart';
import 'package:football_gm_app/leagues/league_api.dart';
import 'package:football_gm_app/leagues/screens/league_information_screen.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/arcade_submit_button.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';
import 'package:football_gm_app/ui/widgets/status_banner.dart';
import 'package:provider/provider.dart';

class JoinLeagueScreen extends StatefulWidget {
  const JoinLeagueScreen({super.key});

  @override
  State<JoinLeagueScreen> createState() => _JoinLeagueScreenState();
}

class _JoinLeagueScreenState extends State<JoinLeagueScreen> {
  final _formKey = GlobalKey<FormState>();
  final _joinCode = TextEditingController();
  bool _busy = false;
  String? _error;

  @override
  void dispose() {
    _joinCode.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    setState(() => _error = null);
    if (!(_formKey.currentState?.validate() ?? false)) return;

    setState(() => _busy = true);
    try {
      final api = context.read<LeagueApi>();
      final leagueId = await api.joinLeague(_joinCode.text.trim());
      if (!mounted) return;
      Navigator.of(context).pushReplacement(
        MaterialPageRoute<void>(
          builder: (_) =>
              LeagueInformationScreen(leagueId: leagueId, leagueApi: api),
        ),
      );
    } on UnknownJoinCodeException {
      if (!mounted) return;
      setState(() {
        _busy = false;
        _error = 'Unknown Join code';
      });
    } on Object {
      if (!mounted) return;
      setState(() {
        _busy = false;
        _error = 'Could not join League';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return ArcadePage(
      title: 'Join with code',
      maxWidth: 440,
      body: PixelPanel(
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (_error != null) StatusBanner(text: _error!),
                TextFormField(
                  controller: _joinCode,
                  enabled: !_busy,
                  textCapitalization: TextCapitalization.characters,
                  decoration: const InputDecoration(labelText: 'Join code'),
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Join code is required';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 24),
                ArcadeSubmitButton(
                  label: 'Join',
                  busy: _busy,
                  onPressed: _submit,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
