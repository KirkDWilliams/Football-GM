import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/arcade_assets.dart';
import 'package:football_gm_app/ui/arcade_colors.dart';

/// Stadium plate + CRT scanlines sitting behind every page.
class FieldBackdrop extends StatelessWidget {
  const FieldBackdrop({super.key});

  @override
  Widget build(BuildContext context) {
    return const IgnorePointer(
      child: Stack(
        fit: StackFit.expand,
        children: [
          ColoredBox(color: ArcadeColors.turfDeep),
          _StadiumPlate(),
          ColoredBox(color: Color(0x5907140C)),
          CustomPaint(painter: _ScanlinePainter()),
          CustomPaint(painter: _VignettePainter()),
        ],
      ),
    );
  }
}

class _StadiumPlate extends StatelessWidget {
  const _StadiumPlate();

  @override
  Widget build(BuildContext context) {
    return Image.asset(
      ArcadeAssets.stadium,
      fit: BoxFit.cover,
      alignment: const Alignment(0, -0.2),
      filterQuality: FilterQuality.none,
      errorBuilder: (_, _, _) => const SizedBox.shrink(),
    );
  }
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
      ..shader = RadialGradient(
        colors: const [Colors.transparent, Color(0x99000000)],
        stops: const [0.55, 1],
      ).createShader(rect);
    canvas.drawRect(rect, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
