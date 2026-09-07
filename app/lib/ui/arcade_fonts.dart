import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';

/// Pixel title face (Press Start 2P) and readable body face (VT323).
abstract final class ArcadeFonts {
  static const titleFamily = 'PressStart2P';
  static const bodyFamily = 'VT323';

  static TextStyle title({
    double size = 12,
    Color color = ArcadeColors.cream,
    double height = 1.7,
  }) {
    return _style(
      family: titleFamily,
      size: size,
      color: color,
      height: height,
    );
  }

  static TextStyle body({
    double size = 20,
    Color color = ArcadeColors.cream,
    double height = 1.15,
  }) {
    return _style(family: bodyFamily, size: size, color: color, height: height);
  }

  static TextStyle _style({
    required String family,
    required double size,
    required Color color,
    required double height,
  }) {
    return TextStyle(
      fontFamily: family,
      fontSize: size,
      color: color,
      height: height,
      fontWeight: FontWeight.w400,
    );
  }
}
