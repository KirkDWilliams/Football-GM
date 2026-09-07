import 'package:flutter/material.dart';
import 'package:football_gm_app/auth/auth_controller.dart';
import 'package:football_gm_app/auth/screens/register_screen.dart';
import 'package:football_gm_app/navigation/app_section.dart';
import 'package:football_gm_app/navigation/navigation_controller.dart';
import 'package:football_gm_app/ui/widgets/arcade_page.dart';
import 'package:football_gm_app/ui/widgets/arcade_password_field.dart';
import 'package:football_gm_app/ui/widgets/arcade_submit_button.dart';
import 'package:football_gm_app/ui/widgets/pixel_panel.dart';
import 'package:football_gm_app/ui/widgets/status_banner.dart';
import 'package:provider/provider.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _email = TextEditingController();
  final _password = TextEditingController();
  bool _hidePassword = true;

  @override
  void dispose() {
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final auth = context.read<AuthController>();
    auth.clearError();
    if (!(_formKey.currentState?.validate() ?? false)) return;

    final ok = await auth.login(
      email: _email.text.trim(),
      password: _password.text,
    );
    if (ok && mounted) {
      context.read<NavigationController>().go(AppSection.home, context);
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthController>();

    return ArcadePage(
      centerBody: true,
      maxWidth: 440,
      body: PixelPanel(
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  'Sign in',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineMedium,
                ),
                const SizedBox(height: 24),
                if (auth.successMessage != null)
                  StatusBanner(
                    text: auth.successMessage!,
                    error: false,
                    onClose: auth.clearSuccessMessage,
                  ),
                if (auth.errorMessage != null)
                  StatusBanner(
                    text: auth.errorMessage!,
                    onClose: auth.clearError,
                  ),
                TextFormField(
                  controller: _email,
                  keyboardType: TextInputType.emailAddress,
                  enabled: !auth.busy,
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
                  obscure: _hidePassword,
                  onToggle: () =>
                      setState(() => _hidePassword = !_hidePassword),
                  enabled: !auth.busy,
                  onFieldSubmitted: _submit,
                  validator: (v) =>
                      (v == null || v.isEmpty) ? 'Password is required' : null,
                ),
                const SizedBox(height: 24),
                ArcadeSubmitButton(
                  label: 'Sign in',
                  busy: auth.busy,
                  onPressed: _submit,
                ),
                TextButton(
                  onPressed: auth.busy
                      ? null
                      : () {
                          auth.clearError();
                          Navigator.of(context).push(
                            MaterialPageRoute<void>(
                              builder: (_) => const RegisterScreen(),
                            ),
                          );
                        },
                  child: const Text('Create an account'),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
