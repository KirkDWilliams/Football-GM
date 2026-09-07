import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/arcade_password_field.dart';
import 'package:football_gm_app/ui/widgets/arcade_submit_button.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';
import 'package:football_gm_app/ui/widgets/status_banner.dart';
import 'package:provider/provider.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  bool _hidePassword = true;

  @override
  void dispose() {
    _name.dispose();
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final auth = context.read<AuthController>();
    auth.clearError();
    if (!(_formKey.currentState?.validate() ?? false)) return;

    final ok = await auth.register(
      email: _email.text.trim(),
      password: _password.text,
      displayName: _name.text.trim(),
    );

    if (ok && mounted) {
      context.read<NavigationController>().go(AppSection.home, context);
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthController>();

    return ArcadePage(
      title: 'Create account',
      maxWidth: 440,
      body: PixelPanel(
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                if (auth.errorMessage != null)
                  StatusBanner(
                    text: auth.errorMessage!,
                    onClose: auth.clearError,
                  ),
                TextFormField(
                  controller: _name,
                  enabled: !auth.busy,
                  textCapitalization: TextCapitalization.words,
                  decoration: const InputDecoration(labelText: 'Display name'),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Display name is required';
                    }
                    if (v.trim().length > 100) {
                      return 'Max 100 characters';
                    }
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                TextFormField(
                  controller: _email,
                  enabled: !auth.busy,
                  keyboardType: TextInputType.emailAddress,
                  decoration: const InputDecoration(labelText: 'Email'),
                  validator: (v) {
                    if (v == null || v.trim().isEmpty) {
                      return 'Email is required';
                    }
                    if (!v.contains('@')) return 'Enter a valid email';
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                ArcadePasswordField(
                  controller: _password,
                  label: 'Password',
                  helperText: 'At least 8 characters',
                  obscure: _hidePassword,
                  onToggle: () =>
                      setState(() => _hidePassword = !_hidePassword),
                  enabled: !auth.busy,
                  onFieldSubmitted: _submit,
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Password is required';
                    if (v.length < 8) return 'At least 8 characters';
                    return null;
                  },
                ),
                const SizedBox(height: 24),
                ArcadeSubmitButton(
                  label: 'Create account',
                  busy: auth.busy,
                  onPressed: _submit,
                ),
                TextButton(
                  onPressed: auth.busy
                      ? null
                      : () {
                          auth.clearError();
                          Navigator.of(context).pop();
                        },
                  child: const Text('Already have an account? Sign in'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
