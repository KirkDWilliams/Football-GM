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
    return TextStyle(
      fontFamily: titleFamily,
      fontSize: size,
      color: color,
      height: height,
      fontWeight: FontWeight.w400,
    );
  }

  static TextStyle body({
    double size = 20,
    Color color = ArcadeColors.cream,
    double height = 1.15,
  }) {
    return TextStyle(
      fontFamily: bodyFamily,
      fontSize: size,
      color: color,
      height: height,
      fontWeight: FontWeight.w400,
    );
  }
}
