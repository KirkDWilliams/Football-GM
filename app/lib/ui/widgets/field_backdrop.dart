import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';

/// Chunky 16-bit field + CRT scanlines sitting behind every page.
class FieldBackdrop extends StatelessWidget {
  const FieldBackdrop({super.key});

  @override
  Widget build(BuildContext context) {
    return const IgnorePointer(
      child: Stack(
        fit: StackFit.expand,
        children: [
          ColoredBox(color: ArcadeColors.turfDeep),
          CustomPaint(painter: PixelFieldPainter()),
          CustomPaint(painter: _ScanlinePainter()),
          CustomPaint(painter: _VignettePainter()),
        ],
      ),
    );
  }
}

class PixelFieldPainter extends CustomPainter {
  const PixelFieldPainter();

  static const _logical = Size(160, 100);

  @override
  void paint(Canvas canvas, Size size) {
    final scale = math.max(
      size.width / _logical.width,
      size.height / _logical.height,
    );
    canvas
      ..save()
      ..translate(
        (size.width - _logical.width * scale) / 2,
        (size.height - _logical.height * scale) / 2,
      )
      ..scale(scale);
    _paintLogical(canvas);
    canvas.restore();
  }

  void _paintLogical(Canvas canvas) {
    const w = 160.0;
    canvas.drawRect(
      const Rect.fromLTWH(0, 0, w, 26),
      Paint()..color = const Color(0xFF0B1220),
    );

    final star = Paint()..color = const Color(0xFFE8E0C0);
    for (final p in const [
      Offset(8, 4),
      Offset(22, 9),
      Offset(41, 3),
      Offset(63, 7),
      Offset(88, 5),
      Offset(109, 10),
      Offset(131, 3),
      Offset(148, 8),
    ]) {
      canvas.drawRect(Rect.fromLTWH(p.dx, p.dy, 1, 1), star);
    }

    final light = Paint()..color = ArcadeColors.gold;
    canvas.drawRect(const Rect.fromLTWH(20, 14, 6, 3), light);
    canvas.drawRect(const Rect.fromLTWH(134, 14, 6, 3), light);
    final beam = Paint()..color = const Color(0x33F4C430);
    canvas.drawPath(
      Path()
        ..moveTo(20, 17)
        ..lineTo(38, 26)
        ..lineTo(14, 26)
        ..close(),
      beam,
    );
    canvas.drawPath(
      Path()
        ..moveTo(140, 17)
        ..lineTo(146, 26)
        ..lineTo(122, 26)
        ..close(),
      beam,
    );

    canvas.drawRect(
      const Rect.fromLTWH(0, 26, 22, 62),
      Paint()..color = const Color(0xFF16162A),
    );
    canvas.drawRect(
      const Rect.fromLTWH(138, 26, 22, 62),
      Paint()..color = const Color(0xFF16162A),
    );
    final row = Paint()..color = const Color(0xFF22223A);
    for (var y = 28.0; y < 86; y += 4) {
      canvas.drawRect(Rect.fromLTWH(1, y, 20, 2), row);
      canvas.drawRect(Rect.fromLTWH(139, y, 20, 2), row);
    }

    canvas.drawRect(
      const Rect.fromLTWH(22, 26, 116, 62),
      Paint()..color = ArcadeColors.turfMid,
    );
    final stripe = Paint()..color = const Color(0xFF237A2A);
    for (var y = 32.0; y < 82; y += 8) {
      canvas.drawRect(Rect.fromLTWH(22, y, 116, 4), stripe);
    }

    final endzone = Paint()..color = ArcadeColors.endzone;
    canvas.drawRect(const Rect.fromLTWH(22, 26, 116, 6), endzone);
    canvas.drawRect(const Rect.fromLTWH(22, 82, 116, 6), endzone);

    final line = Paint()..color = ArcadeColors.cream;
    for (var y = 38.0; y <= 76; y += 8) {
      canvas.drawRect(Rect.fromLTWH(22, y, 116, 1), line);
    }
    canvas.drawRect(const Rect.fromLTWH(22, 26, 1, 62), line);
    canvas.drawRect(const Rect.fromLTWH(137, 26, 1, 62), line);
    canvas.drawRect(const Rect.fromLTWH(79, 32, 2, 50), line);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _ScanlinePainter extends CustomPainter {
  const _ScanlinePainter();

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = ArcadeColors.scanline;
    for (var y = 0.0; y < size.height; y += 3) {
      canvas.drawRect(Rect.fromLTWH(0, y, size.width, 1), paint);
    }
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}

class _VignettePainter extends CustomPainter {
  const _VignettePainter();

  @override
  void paint(Canvas canvas, Size size) {
    final rect = Offset.zero & size;
    final paint = Paint()
      ..shader = const RadialGradient(
        colors: [Colors.transparent, Color(0x99000000)],
        stops: [0.55, 1],
      ).createShader(rect);
    canvas.drawRect(rect, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
