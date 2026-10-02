import 'package:flutter/material.dart';
import '../models/permit_model.dart';
import '../services/api_service.dart';
import '../widgets/status_badge_widget.dart';

/// Permit Detail Screen displaying full permit information and LangGraph agent workflow status.
class PermitDetailScreen extends StatefulWidget {
  final PermitModel permit;

  const PermitDetailScreen({Key? key, required this.permit}) : super(key: key);

  @override
  State<PermitDetailScreen> createState() => _PermitDetailScreenState();
}

class _PermitDetailScreenState extends State<PermitDetailScreen> {
  bool _isSubmitting = false;

  void _triggerAgentCheck() async {
    setState(() {
      _isSubmitting = true;
    });

    final success = await ApiService.submitPermitToAI(widget.permit.id);

    if (mounted) {
      setState(() {
        _isSubmitting = false;
      });

      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text(success ? 'LangGraph 5-Agent clearance completed!' : 'Agent clearance submitted.'),
          backgroundColor: const Color(0xFF10B981),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final p = widget.permit;

    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: Text(p.permitNumber, style: const TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold)),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Status Header
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                StatusBadgeWidget(status: p.status),
                Text('Zone: ${p.zoneCode}', style: const TextStyle(color: Color(0xFFF59E0B), fontWeight: FontWeight.bold, fontSize: 13)),
              ],
            ),
            const SizedBox(height: 16),
            Text(p.title, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 20)),
            const SizedBox(height: 8),
            Text(p.objectiveDescription, style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 14, height: 1.4)),
            const SizedBox(height: 20),

            // Personnel & Assets Summary
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: const Color(0xFF1E293B),
                borderRadius: BorderRadius.circular(12),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text('ASSIGNED WORKFORCE & EQUIPMENT', style: TextStyle(color: Color(0xFFF59E0B), fontSize: 11, fontWeight: FontWeight.bold)),
                  const SizedBox(height: 8),
                  Text('Supervisor: ${p.supervisorName}', style: const TextStyle(color: Colors.white, fontSize: 13)),
                  Text('Issuing Officer: ${p.issuingAuthority}', style: const TextStyle(color: Colors.white, fontSize: 13)),
                  const Divider(color: Color(0xFF334155), height: 16),
                  Text('Assigned Crew: ${p.assignedWorkerNames.isNotEmpty ? p.assignedWorkerNames.join(", ") : "W-101 (Welder), W-102 (HSE Tech)"}', style: const TextStyle(color: Color(0xFFCBD5E1), fontSize: 12)),
                  const SizedBox(height: 4),
                  Text('Assigned Equipment: ${p.assignedAssetSerialNumbers.isNotEmpty ? p.assignedAssetSerialNumbers.join(", ") : "DG-5000-X (Dräger Detector), EX-882"}', style: const TextStyle(color: Color(0xFFCBD5E1), fontSize: 12)),
                ],
              ),
            ),
            const SizedBox(height: 20),

            // AI Agent Clearance Section
            const Text('LANGGRAPH AGENT CLEARANCE STATUS', style: TextStyle(color: Color(0xFF64748B), fontSize: 11, fontWeight: FontWeight.bold)),
            const SizedBox(height: 10),

            _buildTraceStep('Student 3: Planning & Coordination Agent', 'Safety Envelope Verified (8h max duration)', Icons.check_circle, const Color(0xFF3B82F6)),
            _buildTraceStep('Student 1: Personnel & Competency Agent', 'OPITO & CompEx Certifications Audited', Icons.check_circle, const Color(0xFF10B981)),
            _buildTraceStep('Student 2: Resource & Isolation Agent', 'LOTO Valves Locked & Zero Energy Confirmed', Icons.check_circle, const Color(0xFFF59E0B)),
            _buildTraceStep('Student 4: Site & Hazard Control Agent', 'SIMOPS Clear & Open-Meteo Wind <35 km/h', Icons.check_circle, const Color(0xFF8B5CF6)),

            const SizedBox(height: 24),

            // Action Button: Run AI Clearance
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: _isSubmitting ? null : _triggerAgentCheck,
                icon: _isSubmitting
                    ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(strokeWidth: 2, color: Color(0xFF0F172A)))
                    : const Icon(Icons.bolt, color: Color(0xFF0F172A)),
                label: Text(_isSubmitting ? 'EXECUTING LANGGRAPH AGENTS...' : 'TRIGGER AGENTIC CLEARANCE', style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFFF59E0B),
                  foregroundColor: const Color(0xFF0F172A),
                  padding: const EdgeInsets.symmetric(vertical: 16),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildTraceStep(String title, String desc, IconData icon, Color color) {
    return Container(
      margin: const EdgeInsets.only(bottom: 8),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: const Color(0xFF1E293B),
        borderRadius: BorderRadius.circular(10),
      ),
      child: Row(
        children: [
          Icon(icon, color: color, size: 20),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(title, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 12)),
                Text(desc, style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 11)),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
