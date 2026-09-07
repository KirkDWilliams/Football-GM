import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/arcade_assets.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';

/// Title screen. Intentionally empty of product UI so later seasons can land here.
class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    final text = Theme.of(context).textTheme;

    return ArcadePage(
      centerBody: true,
      maxWidth: 560,
      body: PixelPanel(
        padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 36),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset(
              ArcadeAssets.football,
              height: 120,
              filterQuality: FilterQuality.none,
            ),
            const SizedBox(height: 28),
            Text(
              'FOOTBALL GM',
              textAlign: TextAlign.center,
              style: text.displayMedium,
            ),
            const SizedBox(height: 16),
            Text(
              'A retro fantasy football sim.',
              textAlign: TextAlign.center,
              style: text.bodyLarge?.copyWith(color: ArcadeColors.creamMuted),
            ),
          ],
        ),
      ),
    );
  }
}
