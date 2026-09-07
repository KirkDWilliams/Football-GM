import 'package:flutter/material.dart';

/// Dismissible status strip for form errors and success copy.
class StatusBanner extends StatelessWidget {
  const StatusBanner({
    super.key,
    required this.text,
    this.error = true,
    this.onClose,
  });

  final String text;
  final bool error;
  final VoidCallback? onClose;

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final background = error ? scheme.errorContainer : scheme.primaryContainer;
    final foreground = error
        ? scheme.onErrorContainer
        : scheme.onPrimaryContainer;

    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: Material(
        color: background,
        child: ListTile(
          title: Text(
            text,
            style: Theme.of(
              context,
            ).textTheme.bodyMedium?.copyWith(color: foreground),
          ),
          trailing: onClose == null
              ? null
              : IconButton(
                  icon: const Icon(Icons.close),
                  color: foreground,
                  onPressed: onClose,
                ),
        ),
      ),
    );
  }
}
