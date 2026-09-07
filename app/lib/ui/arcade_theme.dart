import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';
import 'package:football_gm_app/ui/arcade_fonts.dart';

/// Material 3 theme tuned for a 16-bit football sim: sharp corners, gold on turf.
abstract final class ArcadeTheme {
  static ThemeData dark() {
    const scheme = ColorScheme(
      brightness: Brightness.dark,
      primary: ArcadeColors.gold,
      onPrimary: ArcadeColors.turfDeep,
      secondary: ArcadeColors.turfMid,
      onSecondary: ArcadeColors.cream,
      secondaryContainer: ArcadeColors.turfMid,
      onSecondaryContainer: ArcadeColors.gold,
      tertiary: ArcadeColors.endzone,
      onTertiary: ArcadeColors.cream,
      error: Color(0xFFFF8A80),
      onError: ArcadeColors.turfDeep,
      errorContainer: Color(0xFF4A1C1C),
      onErrorContainer: ArcadeColors.cream,
      primaryContainer: ArcadeColors.turfMid,
      onPrimaryContainer: ArcadeColors.gold,
      surface: ArcadeColors.panel,
      onSurface: ArcadeColors.cream,
      outline: ArcadeColors.gold,
      outlineVariant: ArcadeColors.goldDim,
      surfaceContainerHighest: ArcadeColors.turf,
    );

    const zero = RoundedRectangleBorder();

    return ThemeData(
      useMaterial3: true,
      brightness: Brightness.dark,
      colorScheme: scheme,
      fontFamily: ArcadeFonts.bodyFamily,
      scaffoldBackgroundColor: Colors.transparent,
      canvasColor: ArcadeColors.panel,
      splashFactory: NoSplash.splashFactory,
      highlightColor: ArcadeColors.gold.withValues(alpha: 0.12),
      hoverColor: ArcadeColors.gold.withValues(alpha: 0.08),
      dividerColor: ArcadeColors.goldDim,
      textTheme: TextTheme(
        displayLarge: ArcadeFonts.title(size: 22, color: ArcadeColors.gold),
        displayMedium: ArcadeFonts.title(size: 18, color: ArcadeColors.gold),
        headlineMedium: ArcadeFonts.title(size: 14, color: ArcadeColors.gold),
        headlineSmall: ArcadeFonts.title(size: 12, color: ArcadeColors.gold),
        titleLarge: ArcadeFonts.title(size: 12),
        titleMedium: ArcadeFonts.title(size: 10),
        titleSmall: ArcadeFonts.title(size: 8, color: ArcadeColors.gold),
        bodyLarge: ArcadeFonts.body(size: 22),
        bodyMedium: ArcadeFonts.body(size: 20),
        bodySmall: ArcadeFonts.body(size: 18, color: ArcadeColors.creamMuted),
        labelLarge: ArcadeFonts.body(size: 22, color: ArcadeColors.turfDeep),
        labelMedium: ArcadeFonts.title(size: 8),
        labelSmall: ArcadeFonts.body(size: 16, color: ArcadeColors.creamMuted),
      ),
      appBarTheme: AppBarTheme(
        backgroundColor: Colors.transparent,
        foregroundColor: ArcadeColors.cream,
        elevation: 0,
        scrolledUnderElevation: 0,
        centerTitle: false,
        titleTextStyle: ArcadeFonts.title(size: 12, color: ArcadeColors.gold),
        iconTheme: const IconThemeData(color: ArcadeColors.gold, size: 22),
        systemOverlayStyle: SystemUiOverlayStyle.light,
      ),
      filledButtonTheme: FilledButtonThemeData(
        style: FilledButton.styleFrom(
          shape: zero,
          elevation: 0,
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
          textStyle: ArcadeFonts.body(size: 22),
        ),
      ),
      outlinedButtonTheme: OutlinedButtonThemeData(
        style: OutlinedButton.styleFrom(
          foregroundColor: ArcadeColors.gold,
          shape: zero,
          side: const BorderSide(color: ArcadeColors.gold, width: 2),
          padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
          textStyle: ArcadeFonts.body(size: 22, color: ArcadeColors.gold),
        ),
      ),
      textButtonTheme: TextButtonThemeData(
        style: TextButton.styleFrom(
          foregroundColor: ArcadeColors.gold,
          shape: zero,
          textStyle: ArcadeFonts.body(size: 20, color: ArcadeColors.gold),
        ),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: ArcadeColors.turfDeep,
        labelStyle: ArcadeFonts.body(size: 18, color: ArcadeColors.creamMuted),
        helperStyle: ArcadeFonts.body(size: 16, color: ArcadeColors.creamMuted),
        errorStyle: ArcadeFonts.body(size: 16, color: scheme.error),
        floatingLabelStyle: ArcadeFonts.body(
          size: 18,
          color: ArcadeColors.gold,
        ),
        border: _inputBorder(ArcadeColors.goldDim),
        enabledBorder: _inputBorder(ArcadeColors.goldDim),
        focusedBorder: _inputBorder(ArcadeColors.gold, width: 2),
        errorBorder: _inputBorder(scheme.error),
        focusedErrorBorder: _inputBorder(scheme.error, width: 2),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 14,
          vertical: 14,
        ),
      ),
      cardTheme: const CardThemeData(
        color: ArcadeColors.panel,
        elevation: 0,
        margin: EdgeInsets.symmetric(vertical: 6),
        shape: RoundedRectangleBorder(
          side: BorderSide(color: ArcadeColors.goldDim, width: 2),
        ),
      ),
      chipTheme: ChipThemeData(
        backgroundColor: ArcadeColors.turf,
        selectedColor: ArcadeColors.turfMid,
        labelStyle: ArcadeFonts.body(size: 18),
        side: const BorderSide(color: ArcadeColors.goldDim, width: 2),
        shape: zero,
        padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
      ),
      snackBarTheme: SnackBarThemeData(
        backgroundColor: ArcadeColors.gold,
        contentTextStyle: ArcadeFonts.body(
          size: 20,
          color: ArcadeColors.turfDeep,
        ),
        shape: zero,
        behavior: SnackBarBehavior.floating,
        elevation: 0,
      ),
      dialogTheme: const DialogThemeData(
        backgroundColor: ArcadeColors.panel,
        shape: RoundedRectangleBorder(
          side: BorderSide(color: ArcadeColors.gold, width: 2),
        ),
      ),
      popupMenuTheme: PopupMenuThemeData(
        color: ArcadeColors.panel,
        elevation: 0,
        textStyle: ArcadeFonts.body(),
        shape: const RoundedRectangleBorder(
          side: BorderSide(color: ArcadeColors.gold, width: 2),
        ),
      ),
      listTileTheme: ListTileThemeData(
        iconColor: ArcadeColors.gold,
        textColor: ArcadeColors.cream,
        titleTextStyle: ArcadeFonts.body(size: 22),
        subtitleTextStyle: ArcadeFonts.body(
          size: 18,
          color: ArcadeColors.creamMuted,
        ),
      ),
      progressIndicatorTheme: const ProgressIndicatorThemeData(
        color: ArcadeColors.gold,
        circularTrackColor: ArcadeColors.turf,
      ),
      iconTheme: const IconThemeData(color: ArcadeColors.gold),
      dividerTheme: const DividerThemeData(
        color: ArcadeColors.goldDim,
        thickness: 2,
        space: 24,
      ),
      tooltipTheme: TooltipThemeData(
        decoration: BoxDecoration(
          color: ArcadeColors.turfDeep,
          border: Border.all(color: ArcadeColors.gold, width: 2),
        ),
        textStyle: ArcadeFonts.body(size: 16, color: ArcadeColors.cream),
      ),
    );
  }

  static OutlineInputBorder _inputBorder(Color color, {double width = 2}) {
    return OutlineInputBorder(
      borderRadius: BorderRadius.zero,
      borderSide: BorderSide(color: color, width: width),
    );
  }
}
