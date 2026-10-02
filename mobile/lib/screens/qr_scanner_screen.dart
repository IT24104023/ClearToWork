import 'package:flutter/material.dart';

/// Device Feature Screen: QR Code Badge & LOTO Tag Camera Scanner.
class QRScannerScreen extends StatefulWidget {
  const QRScannerScreen({Key? key}) : super(key: key);

  @override
  State<QRScannerScreen> createState() => _QRScannerScreenState();
}

class _QRScannerScreenState extends State<QRScannerScreen> {
  String? _scannedData;
  bool _isScanning = true;

  void _simulateScan(String data) {
    setState(() {
      _scannedData = data;
      _isScanning = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: const Text('Digital Badge & LOTO QR Scanner', style: TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold)),
      ),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            children: [
              const Text(
                'ALIGN BADGE QR CODE WITHIN FRAME',
                style: TextStyle(color: Color(0xFFF59E0B), fontSize: 11, fontWeight: FontWeight.bold, letterSpacing: 0.8),
              ),
              const SizedBox(height: 20),

              // Camera Viewfinder Simulation
              Expanded(
                child: Container(
                  decoration: BoxDecoration(
                    color: const Color(0xFF1E293B),
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: const Color(0xFFF59E0B), width: 2),
                  ),
                  child: Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(
                          _isScanning ? Icons.qr_code_scanner : Icons.check_circle_outline,
                          size: 100,
                          color: _isScanning ? const Color(0xFFF59E0B) : const Color(0xFF10B981),
                        ),
                        const SizedBox(height: 16),
                        Text(
                          _isScanning ? 'Camera Scanner Active...' : 'QR Code Successfully Scanned!',
                          style: TextStyle(color: _isScanning ? const Color(0xFF94A3B8) : const Color(0xFF10B981), fontWeight: FontWeight.bold),
                        ),
                      ],
                    ),
                  ),
                ),
              ),

              const SizedBox(height: 20),

              if (_scannedData != null) ...[
                Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: const Color(0xFF10B981).withOpacity(0.15),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: const Color(0xFF10B981)),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('SCANNED BADGE / TAG PAYLOAD:', style: TextStyle(color: Color(0xFF10B981), fontWeight: FontWeight.bold, fontSize: 11)),
                      const SizedBox(height: 6),
                      Text(_scannedData!, style: const TextStyle(color: Colors.white, fontSize: 13, fontFamily: 'monospace')),
                    ],
                  ),
                ),
                const SizedBox(height: 16),
              ],

              // Fast Scan Preset Buttons for Viva Demo
              const Text('TAP PRESET TO SIMULATE QR SCAN', style: TextStyle(color: Color(0xFF64748B), fontSize: 11, fontWeight: FontWeight.bold)),
              const SizedBox(height: 10),
              Row(
                children: [
                  Expanded(
                    child: ElevatedButton(
                      onPressed: () => _simulateScan('WORKER:W-101 | ROLE:Pipefitter | OPITO:VALID'),
                      style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF334155)),
                      child: const Text('Worker Badge W-101', style: TextStyle(color: Colors.white, fontSize: 11)),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: () => _simulateScan('LOTO:TAG-8821 | ZONE:ZONE-A1 | LOCK:SECURED'),
                      style: ElevatedButton.styleFrom(backgroundColor: const Color(0xFF334155)),
                      child: const Text('LOTO Lock Tag 8821', style: TextStyle(color: Colors.white, fontSize: 11)),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
