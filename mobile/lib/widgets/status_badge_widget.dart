import 'package:flutter/material.dart';

/// Reusable status badge widget displaying permit states with industrial color schemes.
class StatusBadgeWidget extends StatelessWidget {
  final String status;

  const StatusBadgeWidget({Key? key, required this.status}) : super(key: key);

  Color _getBackgroundColor() {
    switch (status) {
      case 'Active':
        return const Color(0xFF1E3A8A); // Deep Blue
      case 'Approved':
        return const Color(0xFF065F46); // Emerald
      case 'Refused':
        return const Color(0xFF881337); // Dark Rose
      case 'PendingApproval':
      case 'AiReview':
        return const Color(0xFF78350F); // Amber / Purple
      case 'Draft':
      default:
        return const Color(0xFF334155); // Slate
    }
  }

  Color _getTextColor() {
    switch (status) {
      case 'Active':
        return const Color(0xFF93C5FD);
      case 'Approved':
        return const Color(0xFF6EE7B7);
      case 'Refused':
        return const Color(0xFFFDA4AF);
      case 'PendingApproval':
      case 'AiReview':
        return const Color(0xFFFDE68A);
      case 'Draft':
      default:
        return const Color(0xFFCBD5E1);
    }
  }

  String _getLabelText() {
    switch (status) {
      case 'Active':
        return 'ACTIVE ON SITE';
      case 'Approved':
        return 'APPROVED';
      case 'Refused':
        return 'REFUSED (SAFE FAILURE)';
      case 'PendingApproval':
        return 'PENDING SIGN-OFF';
      case 'AiReview':
        return 'AI AGENTS REVIEWING';
      case 'Draft':
        return 'DRAFT';
      default:
        return status.toUpperCase();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: _getBackgroundColor(),
        borderRadius: BorderRadius.circular(6),
        border: Border.all(color: _getTextColor().withOpacity(0.4), width: 1),
      ),
      child: Text(
        _getLabelText(),
        style: TextStyle(
          color: _getTextColor(),
          fontSize: 10,
          fontWeight: FontWeight.bold,
          letterSpacing: 0.5,
        ),
      ),
    );
  }
}
