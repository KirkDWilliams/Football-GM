/// Top-level destinations in the persistent website nav.
enum AppSection {
  home,
  leagues,
  login;

  String get label => switch (this) {
    home => 'Home',
    leagues => 'Leagues',
    login => 'Login',
  };

  /// Login is omitted while signed in; the account menu takes its place.
  static List<AppSection> destinations({required bool signedIn}) => [
    home,
    leagues,
    if (!signedIn) login,
  ];
}
