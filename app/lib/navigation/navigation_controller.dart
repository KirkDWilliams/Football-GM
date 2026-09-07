import 'package:flutter/foundation.dart';
import 'package:football_gm_app/navigation/app_section.dart';

/// Which top-level section the shell is showing.
class NavigationController extends ChangeNotifier {
  AppSection section = AppSection.home;

  void go(AppSection next) {
    if (section == next) return;
    section = next;
    notifyListeners();
  }
}
