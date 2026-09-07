import 'package:flutter/material.dart';
import 'package:football_gm_app/ui/ui.dart';

/// Empty root. Product content lands here later; chrome is the nav + field.
class HomePage extends StatelessWidget {
  const HomePage({super.key});

  @override
  Widget build(BuildContext context) {
    return const ArcadePage(body: SizedBox.expand());
  }
}
