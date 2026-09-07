import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';

/// SNES-style beveled panel. Uses [Material] so [ListTile] ink still paints.
class PixelPanel extends StatelessWidget {
  const PixelPanel({
    super.key,
    required this.child,
    this.padding = const EdgeInsets.all(20),
    this.color = ArcadeColors.panel,
  });

  final Widget child;
  final EdgeInsetsGeometry padding;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Material(
      color: color,
      child: Container(
        decoration: const BoxDecoration(
          border: Border(
            top: BorderSide(color: ArcadeColors.bevelLight, width: 3),
            left: BorderSide(color: ArcadeColors.bevelLight, width: 3),
            bottom: BorderSide(color: ArcadeColors.bevelDark, width: 4),
            right: BorderSide(color: ArcadeColors.bevelDark, width: 4),
          ),
        ),
        foregroundDecoration: BoxDecoration(
          border: Border.all(color: Colors.black, width: 2),
        ),
        padding: padding,
        child: child,
      ),
    );
  }
}
