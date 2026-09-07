import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/widgets/field_backdrop.dart';
import 'package:football_gm_app/ui/widgets/game_nav_bar.dart';

/// Page frame for every screen: stadium backdrop, website nav, optional back.
///
/// New routes should wrap their body in this so chrome stays consistent.
class ArcadePage extends StatelessWidget {
  const ArcadePage({
    super.key,
    required this.body,
    this.title,
    this.maxWidth = 960,
    this.centerBody = false,
  });

  final Widget body;
  final String? title;
  final double maxWidth;
  final bool centerBody;

  @override
  Widget build(BuildContext context) {
    final canPop = Navigator.of(context).canPop();
    final header = canPop || title != null
        ? _PageHeader(title: title, showBack: canPop)
        : null;

    return Stack(
      fit: StackFit.expand,
      children: [
        const FieldBackdrop(),
        Scaffold(
          backgroundColor: Colors.transparent,
          body: Column(
            children: [
              const GameNavBar(),
              ?header,
              Expanded(
                child: Align(
                  alignment: centerBody
                      ? Alignment.center
                      : Alignment.topCenter,
                  child: ConstrainedBox(
                    constraints: BoxConstraints(maxWidth: maxWidth),
                    child: Padding(
                      padding: const EdgeInsets.fromLTRB(16, 4, 16, 16),
                      child: body,
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _PageHeader extends StatelessWidget {
  const _PageHeader({required this.title, required this.showBack});

  final String? title;
  final bool showBack;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(4, 0, 16, 0),
      child: Row(
        children: [
          if (showBack) const BackButton(),
          if (title != null)
            Expanded(
              child: Text(
                title!,
                style: Theme.of(context).textTheme.headlineSmall,
              ),
            ),
        ],
      ),
    );
  }
}
