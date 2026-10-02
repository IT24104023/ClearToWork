import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'services/auth_service.dart';
import 'screens/login_screen.dart';
import 'screens/dashboard_screen.dart';

void main() {
  runApp(const ClearToWorkMobileApp());
}

/// Root Application Widget for ClearToWork AI Flutter Mobile Application.
class ClearToWorkMobileApp extends StatelessWidget {
  const ClearToWorkMobileApp({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (_) => AuthProvider(),
      child: Consumer<AuthProvider>(
        builder: (context, auth, _) {
          return MaterialApp(
            title: 'ClearToWork AI Mobile',
            debugShowCheckedModeBanner: false,
            theme: ThemeData(
              brightness: Brightness.dark,
              primaryColor: const Color(0xFFF59E0B),
              scaffoldBackgroundColor: const Color(0xFF0F172A),
              colorScheme: const ColorScheme.dark(
                primary: Color(0xFFF59E0B),
                secondary: Color(0xFF3B82F6),
                surface: Color(0xFF1E293B),
                background: Color(0xFF0F172A),
              ),
              fontFamily: 'sans-serif',
            ),
            home: auth.isAuthenticated ? const DashboardScreen() : const LoginScreen(),
          );
        },
      ),
    );
  }
}
