String? validateEmail(String? value) {
  if (value == null || value.trim().isEmpty) return 'Email is required';
  if (!value.contains('@')) return 'Enter a valid email';
  return null;
}
