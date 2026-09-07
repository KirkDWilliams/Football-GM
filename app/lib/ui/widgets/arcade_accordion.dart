import 'package:flutter/material.dart';

/// Collapsible block used for scoring weights, bonuses, and similar extras.
class ArcadeAccordion extends StatefulWidget {
  const ArcadeAccordion({
    super.key,
    required this.title,
    required this.child,
    this.initiallyOpen = false,
  });

  final String title;
  final Widget child;
  final bool initiallyOpen;

  @override
  State<ArcadeAccordion> createState() => _ArcadeAccordionState();
}

class _ArcadeAccordionState extends State<ArcadeAccordion> {
  late bool _open = widget.initiallyOpen;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ListTile(
          contentPadding: EdgeInsets.zero,
          dense: true,
          title: Text(widget.title),
          trailing: Icon(_open ? Icons.expand_less : Icons.expand_more),
          onTap: () => setState(() => _open = !_open),
        ),
        if (_open) widget.child,
      ],
    );
  }
}
