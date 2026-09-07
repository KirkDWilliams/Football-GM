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

class ChangePasswordScreen extends StatefulWidget {
  const ChangePasswordScreen({super.key});

  @override
  State<ChangePasswordScreen> createState() => _ChangePasswordScreenState();
}

class _ChangePasswordScreenState extends State<ChangePasswordScreen> {
  final _formKey = GlobalKey<FormState>();
  final _current = TextEditingController();
  final _next = TextEditingController();
  final _confirm = TextEditingController();
  bool _hideCurrent = true;
  bool _hideNext = true;
  bool _hideConfirm = true;

  @override
  void dispose() {
    _current.dispose();
    _next.dispose();
    _confirm.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    final auth = context.read<AuthController>();
    auth.clearError();
    if (!(_formKey.currentState?.validate() ?? false)) return;

    final ok = await auth.changePassword(
      currentPassword: _current.text,
      newPassword: _next.text,
    );

    if (ok && mounted) {
      context.read<NavigationController>().go(AppSection.login, context);
    }
  }

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthController>();

    return ArcadePage(
      title: 'Change password',
      maxWidth: 440,
      body: PixelPanel(
        child: Form(
          key: _formKey,
          child: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  'You will need to sign in again on every device.',
                  style: Theme.of(context).textTheme.bodyMedium,
                ),
                const SizedBox(height: 16),
                if (auth.errorMessage != null)
                  StatusBanner(
                    text: auth.errorMessage!,
                    onClose: auth.clearError,
                  ),
                ArcadePasswordField(
                  controller: _current,
                  label: 'Current password',
                  obscure: _hideCurrent,
                  onToggle: () => setState(() => _hideCurrent = !_hideCurrent),
                  enabled: !auth.busy,
                  validator: (v) =>
                      (v == null || v.isEmpty) ? 'Required' : null,
                ),
                const SizedBox(height: 16),
                ArcadePasswordField(
                  controller: _next,
                  label: 'New password',
                  helperText: 'At least 8 characters',
                  obscure: _hideNext,
                  onToggle: () => setState(() => _hideNext = !_hideNext),
                  enabled: !auth.busy,
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Required';
                    if (v.length < 8) return 'At least 8 characters';
                    if (v == _current.text) return 'Must be different';
                    return null;
                  },
                ),
                const SizedBox(height: 16),
                ArcadePasswordField(
                  controller: _confirm,
                  label: 'Confirm new password',
                  obscure: _hideConfirm,
                  onToggle: () => setState(() => _hideConfirm = !_hideConfirm),
                  enabled: !auth.busy,
                  onFieldSubmitted: _submit,
                  validator: (v) {
                    if (v == null || v.isEmpty) return 'Required';
                    if (v != _next.text) return 'Passwords do not match';
                    return null;
                  },
                ),
                const SizedBox(height: 24),
                ArcadeSubmitButton(
                  label: 'Update password',
                  busy: auth.busy,
                  onPressed: _submit,
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
