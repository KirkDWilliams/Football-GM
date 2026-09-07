import 'package:flutter/material.dart';

class ArcadePasswordField extends StatelessWidget {
  const ArcadePasswordField({
    super.key,
    required this.controller,
    required this.label,
    required this.obscure,
    required this.onToggle,
    required this.enabled,
    this.helperText,
    this.validator,
    this.onFieldSubmitted,
  });

  final TextEditingController controller;
  final String label;
  final bool obscure;
  final VoidCallback onToggle;
  final bool enabled;
  final String? helperText;
  final String? Function(String?)? validator;
  final VoidCallback? onFieldSubmitted;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: controller,
      obscureText: obscure,
      enabled: enabled,
      onFieldSubmitted: onFieldSubmitted == null
          ? null
          : (_) => onFieldSubmitted!(),
      decoration: InputDecoration(
        labelText: label,
        helperText: helperText,
        suffixIcon: IconButton(
          icon: Icon(obscure ? Icons.visibility : Icons.visibility_off),
          onPressed: onToggle,
        ),
      ),
      validator: validator,
    );
  }
}
