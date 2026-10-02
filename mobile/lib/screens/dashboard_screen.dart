import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../services/auth_service.dart';
import 'permit_list_screen.dart';
import 'qr_scanner_screen.dart';
import 'gas_monitor_screen.dart';

/// Main Mobile Operational Dashboard for ClearToWork AI.
class DashboardScreen extends StatelessWidget {
  const DashboardScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final auth = Provider.of<AuthProvider>(context);
    final user = auth.user;

    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: Row(
          children: const [
            Icon(Icons.shield, color: Color(0xFFF59E0B)),
            SizedBox(width: 8),
            Text('ClearToWork Mobile', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 16)),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.logout, color: Color(0xFFEF4444)),
            onPressed: () async {
              await auth.logout();
            },
          )
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // User Welcome Header
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [Color(0xFF1E293B), Color(0xFF0F172A)],
                ),
                borderRadius: BorderRadius.circular(16),
                border: Border.all(color: const Color(0xFF334155)),
              ),
              child: Row(
                children: [
                  CircleAvatar(
                    backgroundColor: const Color(0xFFF59E0B),
                    radius: 24,
                    child: Text(
                      user?.fullName.substring(0, 1) ?? 'U',
                      style: const TextStyle(color: Color(0xFF0F172A), fontWeight: FontWeight.bold, fontSize: 20),
                    ),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(user?.fullName ?? 'Authenticated Officer',
                            style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 16)),
                        Text('Role: ${user?.role ?? "Safety Officer"}',
                            style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 12)),
                      ],
                    ),
                  ),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(color: const Color(0xFF10B981).withOpacity(0.2), borderRadius: BorderRadius.circular(8)),
                    child: const Text('API LIVE', style: TextStyle(color: Color(0xFF10B981), fontSize: 10, fontWeight: FontWeight.bold)),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 20),

            const Text(
              'MOBILE FIELD ACTIONS',
              style: TextStyle(color: Color(0xFFF59E0B), fontSize: 11, fontWeight: FontWeight.bold, letterSpacing: 0.8),
            ),
            const SizedBox(height: 12),

            // Quick Device Feature Action Cards (Device QR Scanner, Gas Logger, Permits List)
            Row(
              children: [
                Expanded(
                  child: _buildActionCard(
                    context,
                    title: 'Scan QR Badge',
                    subtitle: 'Device Camera Scan',
                    icon: Icons.qr_code_scanner,
                    color: const Color(0xFF3B82F6),
                    onTap: () {
                      Navigator.of(context).push(MaterialPageRoute(builder: (_) => const QRScannerScreen()));
                    },
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _buildActionCard(
                    context,
                    title: 'Gas Monitoring',
                    subtitle: 'H2S / LEL Telemetry',
                    icon: Icons.air,
                    color: const Color(0xFFEF4444),
                    onTap: () {
                      Navigator.of(context).push(MaterialPageRoute(builder: (_) => const GasMonitorScreen()));
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),

            _buildActionCard(
              context,
              title: 'Safety Permits Register',
              subtitle: 'View Active Permits, AI LangGraph Traces & Approvals',
              icon: Icons.assignment_turned_in,
              color: const Color(0xFF10B981),
              onTap: () {
                Navigator.of(context).push(MaterialPageRoute(builder: (_) => const PermitListScreen()));
              },
            ),

            const SizedBox(height: 24),
            const Text(
              'LANGGRAPH 5-AGENT STATUS',
              style: TextStyle(color: Color(0xFF64748B), fontSize: 11, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 10),

            _buildAgentStatusTile('Student 3: Planning & Coordination', 'ACTIVE (0.8s)', Icons.account_tree, const Color(0xFF3B82F6)),
            _buildAgentStatusTile('Student 1: Personnel & Competencies', 'ACTIVE (Valid Certs)', Icons.badge, const Color(0xFF10B981)),
            _buildAgentStatusTile('Student 2: Equipment & Isolations', 'ACTIVE (LOTO Verified)', Icons.build, const Color(0xFFF59E0B)),
            _buildAgentStatusTile('Student 4: SIMOPS & Weather (Open-Meteo)', 'ACTIVE (<35km/h Wind)', Icons.cloud, const Color(0xFF8B5CF6)),
          ],
        ),
      ),
    );
  }

  Widget _buildActionCard(
    BuildContext context, {
    required String title,
    required String subtitle,
    required IconData icon,
    required Color color,
    required VoidCallback onTap,
  }) {
    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(16),
      child: Container(
        padding: const EdgeInsets.all(16),
        decoration: BoxDecoration(
          color: const Color(0xFF1E293B),
          borderRadius: BorderRadius.circular(16),
          border: Border.all(color: color.withOpacity(0.4), width: 1.5),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Icon(icon, color: color, size: 28),
            const SizedBox(height: 12),
            Text(title, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 14)),
            const SizedBox(height: 4),
            Text(subtitle, style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 11)),
          ],
        ),
      ),
    );
  }

  Widget _buildAgentStatusTile(String title, String status, IconData icon, Color color) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
      decoration: BoxDecoration(
        color: const Color(0xFF1E293B),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Row(
        children: [
          Icon(icon, color: color, size: 18),
          const SizedBox(width: 12),
          Expanded(child: Text(title, style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.w500))),
          Text(status, style: TextStyle(color: color, fontSize: 10, fontWeight: FontWeight.bold)),
        ],
      ),
    );
  }
}
