import 'package:flutter/material.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:provider/provider.dart';

/// Pops stacked routes, then switches the shell section.
void navigateToSection(BuildContext context, AppSection section) {
  Navigator.of(context).popUntil((route) => route.isFirst);
  context.read<NavigationController>().go(section);
}
