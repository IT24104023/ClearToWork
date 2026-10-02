import 'package:flutter/material.dart';
import '../services/api_service.dart';

/// Gas Monitoring Telemetry Logger Screen.
class GasMonitorScreen extends StatefulWidget {
  const GasMonitorScreen({Key? key}) : super(key: key);

  @override
  State<GasMonitorScreen> createState() => _GasMonitorScreenState();
}

class _GasMonitorScreenState extends State<GasMonitorScreen> {
  final _h2sController = TextEditingController(text: '0.0');
  final _lelController = TextEditingController(text: '0.0');
  final _o2Controller = TextEditingController(text: '20.9');
  String _selectedZone = 'ZONE-A1';
  bool _isSending = false;

  void _submitTelemetry() async {
    setState(() {
      _isSending = true;
    });

    final h2s = double.tryParse(_h2sController.text) ?? 0.0;
    final lel = double.tryParse(_lelController.text) ?? 0.0;
    final o2 = double.tryParse(_o2Controller.text) ?? 20.9;

    final success = await ApiService.sendGasTelemetry(
      zoneCode: _selectedZone,
      h2sPpm: h2s,
      lelPercent: lel,
      o2Percent: o2,
    );

    if (mounted) {
      setState(() {
        _isSending = false;
      });

      final bool isSafe = (h2s < 10.0 && lel < 10.0 && o2 >= 19.5 && o2 <= 23.5);

      showDialog(
        context: context,
        builder: (_) => AlertDialog(
          backgroundColor: const Color(0xFF1E293B),
          title: Row(
            children: [
              Icon(isSafe ? Icons.check_circle : Icons.warning_amber_rounded, color: isSafe ? const Color(0xFF10B981) : const Color(0xFFEF4444)),
              const SizedBox(width: 8),
              Text(isSafe ? 'Atmosphere Safe' : 'ATMOSPHERIC HAZARD ALERT', style: const TextStyle(color: Colors.white, fontSize: 16)),
            ],
          ),
          content: Text(
            isSafe
                ? 'Gas telemetry logged to ASP.NET Core API for zone $_selectedZone. All readings within safe OSHA parameters.'
                : 'CRITICAL ALERT: Gas reading violates safety limits! H2S: ${h2s}ppm, LEL: ${lel}%. Immediate evacuation triggered.',
            style: const TextStyle(color: Color(0xFFCBD5E1)),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('OK', style: TextStyle(color: Color(0xFFF59E0B))),
            )
          ],
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: const Text('Portable Multi-Gas Telemetry Logger', style: TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold)),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'ATMOSPHERIC GAS TELEMETRY INPUT',
              style: TextStyle(color: Color(0xFFF59E0B), fontSize: 11, fontWeight: FontWeight.bold, letterSpacing: 0.8),
            ),
            const SizedBox(height: 16),

            DropdownButtonFormField<String>(
              value: _selectedZone,
              dropdownColor: const Color(0xFF1E293B),
              style: const TextStyle(color: Colors.white),
              decoration: InputDecoration(
                labelText: 'Select Hazardous Process Zone',
                labelStyle: const TextStyle(color: Color(0xFF94A3B8)),
                filled: true,
                fillColor: const Color(0xFF1E293B),
                border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
              ),
              items: ['ZONE-A1', 'ZONE-B3', 'ZONE-C2', 'ZONE-D4'].map((z) {
                return DropdownMenuItem(value: z, child: Text(z));
              }).toList(),
              onChanged: (val) {
                if (val != null) setState(() => _selectedZone = val);
              },
            ),
            const SizedBox(height: 16),

            _buildGasField('Hydrogen Sulfide (H2S ppm)', _h2sController, 'Safe threshold: < 10 ppm', Icons.warning_amber),
            const SizedBox(height: 12),
            _buildGasField('Lower Explosive Limit (LEL %)', _lelController, 'Safe threshold: < 10% LEL', Icons.local_fire_department),
            const SizedBox(height: 12),
            _buildGasField('Oxygen Level (O2 %)', _o2Controller, 'Safe range: 19.5% - 23.5%', Icons.air),

            const SizedBox(height: 28),

            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: _isSending ? null : _submitTelemetry,
                icon: const Icon(Icons.cloud_upload, color: Color(0xFF0F172A)),
                label: Text(_isSending ? 'SENDING TELEMETRY...' : 'SUBMIT GAS TELEMETRY TO API', style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 13)),
                style: ElevatedButton.styleFrom(
                  backgroundColor: const Color(0xFFEF4444),
                  foregroundColor: Colors.white,
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

  Widget _buildGasField(String label, TextEditingController controller, String hint, IconData icon) {
    return TextField(
      controller: controller,
      keyboardType: const TextInputType.numberWithOptions(decimal: true),
      style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
      decoration: InputDecoration(
        labelText: label,
        helperText: hint,
        helperStyle: const TextStyle(color: Color(0xFF94A3B8)),
        labelStyle: const TextStyle(color: Color(0xFF94A3B8)),
        prefixIcon: Icon(icon, color: const Color(0xFFF59E0B)),
        filled: true,
        fillColor: const Color(0xFF1E293B),
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(12), borderSide: BorderSide.none),
      ),
    );
  }
}
