import 'package:flutter/material.dart';
import 'package:football_gm_app/navigation/app_section.dart';

/// Which top-level section the shell is showing.
///
/// Sub-pages (create league, register, …) still use [Navigator.push].
/// Calling [go] pops back to the shell so the nav never stacks sections.
class NavigationController extends ChangeNotifier {
  AppSection section = AppSection.home;

  void go(AppSection next, BuildContext context) {
    Navigator.of(context).popUntil((route) => route.isFirst);
    if (section == next) return;
    section = next;
    notifyListeners();
  }
}
