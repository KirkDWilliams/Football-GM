import 'package:flutter/material.dart';

/// Retro turf / stadium palette. Screens should read from [ThemeData]
/// and only reach for these tokens when painting custom chrome.
abstract final class ArcadeColors {
  static const turfDeep = Color(0xFF07140C);
  static const turf = Color(0xFF0F2A18);
  static const turfMid = Color(0xFF1B5E20);
  static const panel = Color(0xFF122418);
  static const gold = Color(0xFFF4C430);
  static const goldDim = Color(0xFFC9A227);
  static const cream = Color(0xFFF5F0D8);
  static const creamMuted = Color(0xFFC9C3A6);
  static const endzone = Color(0xFF8B1E1E);
  static const leather = Color(0xFF6D4C41);
  static const bevelLight = Color(0xFF4CAF50);
  static const bevelDark = Color(0xFF051008);
  static const scanline = Color(0x33000000);
}
