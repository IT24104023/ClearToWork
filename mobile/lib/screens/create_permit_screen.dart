import 'package:flutter/material.dart';
import '../services/api_service.dart';

/// Screen for creating and submitting a new Safety Permit Request directly from the mobile app.
/// Demonstrates cross-platform workflow (Flutter -> ASP.NET Core -> PostgreSQL -> LangGraph AI).
class CreatePermitScreen extends StatefulWidget {
  const CreatePermitScreen({Key? key}) : super(key: key);

  @override
  State<CreatePermitScreen> createState() => _CreatePermitScreenState();
}

class _CreatePermitScreenState extends State<CreatePermitScreen> {
  final _formKey = GlobalKey<FormState>();

  final _titleController = TextEditingController();
  final _objectiveController = TextEditingController();

  String _selectedPermitType = '0c9a435a-7a2d-4b44-a63a-7f81d14db3ed'; // Hot Work
  String _selectedZone = '9f936abb-7739-46c2-86d5-9b158057fb2b'; // Zone A1

  DateTime _startTime = DateTime.now();
  DateTime _endTime = DateTime.now().add(const Duration(hours: 8));

  bool _isGpsVerified = true;
  bool _isSubmitting = false;

  final Map<String, String> _permitTypeOptions = {
    '0c9a435a-7a2d-4b44-a63a-7f81d14db3ed': 'Hot Work (Welding, Cutting, Grinding)',
    '1d8b324a-6b1c-4a33-b52a-6e70d03cb2fc': 'Cold Work (Mechanical Maintenance)',
    '2e7c213b-5a0b-4922-a419-5d69c92ba1eb': 'Confined Space Entry & Vessel Work',
  };

  final Map<String, String> _zoneOptions = {
    '9f936abb-7739-46c2-86d5-9b158057fb2b': 'ZONE-A1: Crude Distillation Unit Deck',
    '8e825baa-6628-45b1-ab84-8a044946ea1a': 'ZONE-B2: Hydrocracker & Gas Tank Farm',
  };

  @override
  void dispose() {
    _titleController.dispose();
    _objectiveController.dispose();
    super.dispose();
  }

  Future<void> _submitForm() async {
    if (!_formKey.currentState!.validate()) return;

    if (!_isGpsVerified) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('On-site GPS verification required before permit submission.'),
          backgroundColor: Color(0xFFEF4444),
        ),
      );
      return;
    }

    setState(() {
      _isSubmitting = true;
    });

    final success = await ApiService.createPermit(
      title: _titleController.text.trim(),
      objectiveDescription: _objectiveController.text.trim(),
      permitTypeId: _selectedPermitType,
      zoneId: _selectedZone,
      startTime: _startTime,
      endTime: _endTime,
    );

    setState(() {
      _isSubmitting = false;
    });

    if (success && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Safety Permit created & submitted to AI agent evaluation!'),
          backgroundColor: Color(0xFF10B981),
        ),
      );
      Navigator.of(context).pop(true);
    } else if (mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Failed to submit permit. Please check network connection.'),
          backgroundColor: Color(0xFFEF4444),
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
        title: const Text('New Safety Permit Request', style: TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold)),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Header Info Card
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: const Color(0xFF1E293B),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(color: const Color(0xFF334155)),
                ),
                child: Row(
                  children: const [
                    Icon(Icons.shield_outlined, color: Color(0xFFF59E0B), size: 24),
                    SizedBox(width: 12),
                    Expanded(
                      child: Text(
                        'Permits submitted here are validated by LangGraph AI agents and synchronized to the laptop Safety Officer dashboard in real time.',
                        style: TextStyle(color: Color(0xFFCBD5E1), fontSize: 12),
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 20),

              // Title Field
              const Text('WORK ACTIVITY TITLE *', style: TextStyle(color: Color(0xFF94A3B8), fontSize: 11, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _titleController,
                style: const TextStyle(color: Colors.white, fontSize: 14),
                decoration: _inputDecoration('e.g. Flange Replacement & Pipe Welding'),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) return 'Please enter work title.';
                  if (val.trim().length < 5) return 'Title must be at least 5 characters.';
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Objective Description Field
              const Text('DETAILED SAFETY OBJECTIVE & SCOPE *', style: TextStyle(color: Color(0xFF94A3B8), fontSize: 11, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              TextFormField(
                controller: _objectiveController,
                maxLines: 3,
                style: const TextStyle(color: Colors.white, fontSize: 14),
                decoration: _inputDecoration('Describe isolation, tools, and hot work procedures...'),
                validator: (val) {
                  if (val == null || val.trim().isEmpty) return 'Please describe the safety objective.';
                  if (val.trim().length < 10) return 'Objective must be at least 10 characters.';
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Permit Type Dropdown
              const Text('PERMIT CLASSIFICATION *', style: TextStyle(color: Color(0xFF94A3B8), fontSize: 11, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 14),
                decoration: BoxDecoration(
                  color: const Color(0xFF1E293B),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: const Color(0xFF334155)),
                ),
                child: DropdownButtonHideUnderline(
                  child: DropdownButton<String>(
                    value: _selectedPermitType,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF1E293B),
                    style: const TextStyle(color: Colors.white, fontSize: 13),
                    items: _permitTypeOptions.entries.map((e) {
                      return DropdownMenuItem(value: e.key, child: Text(e.value));
                    }).toList(),
                    onChanged: (val) {
                      if (val != null) setState(() => _selectedPermitType = val);
                    },
                  ),
                ),
              ),
              const SizedBox(height: 16),

              // Target Zone Dropdown
              const Text('HAZARD ZONE LOCATION *', style: TextStyle(color: Color(0xFF94A3B8), fontSize: 11, fontWeight: FontWeight.bold)),
              const SizedBox(height: 6),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 14),
                decoration: BoxDecoration(
                  color: const Color(0xFF1E293B),
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: const Color(0xFF334155)),
                ),
                child: DropdownButtonHideUnderline(
                  child: DropdownButton<String>(
                    value: _selectedZone,
                    isExpanded: true,
                    dropdownColor: const Color(0xFF1E293B),
                    style: const TextStyle(color: Colors.white, fontSize: 13),
                    items: _zoneOptions.entries.map((e) {
                      return DropdownMenuItem(value: e.key, child: Text(e.value));
                    }).toList(),
                    onChanged: (val) {
                      if (val != null) setState(() => _selectedZone = val);
                    },
                  ),
                ),
              ),
              const SizedBox(height: 20),

              // Device Feature: GPS Geofence Verification Card
              Container(
                padding: const EdgeInsets.all(14),
                decoration: BoxDecoration(
                  color: const Color(0xFF1E293B),
                  borderRadius: BorderRadius.circular(12),
                  border: Border.all(
                    color: _isGpsVerified ? const Color(0xFF10B981) : const Color(0xFFEF4444),
                  ),
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Row(
                          children: const [
                            Icon(Icons.gps_fixed, color: Color(0xFF10B981), size: 18),
                            SizedBox(width: 8),
                            Text('DEVICE GPS GEO-VERIFICATION',
                                style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 12)),
                          ],
                        ),
                        Switch(
                          value: _isGpsVerified,
                          activeColor: const Color(0xFF10B981),
                          onChanged: (val) {
                            setState(() => _isGpsVerified = val);
                          },
                        ),
                      ],
                    ),
                    const SizedBox(height: 6),
                    Text(
                      _isGpsVerified
                          ? 'Device located at Lat: 25.2048° N, Lon: 55.2708° E (Inside Plant Alpha 01 Boundary).'
                          : 'GPS location out of site boundary. Permit cannot be submitted remotely.',
                      style: TextStyle(
                        color: _isGpsVerified ? const Color(0xFF10B981) : const Color(0xFFEF4444),
                        fontSize: 11,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 24),

              // Submit Button
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton.icon(
                  onPressed: _isSubmitting ? null : _submitForm,
                  icon: _isSubmitting
                      ? const SizedBox(width: 18, height: 18, child: CircularProgressIndicator(color: Colors.black, strokeWidth: 2))
                      : const Icon(Icons.send_rounded, color: Color(0xFF0F172A)),
                  label: Text(
                    _isSubmitting ? 'Submitting to AI Engine...' : 'SUBMIT PERMIT REQUEST',
                    style: const TextStyle(color: Color(0xFF0F172A), fontWeight: FontWeight.bold, fontSize: 14),
                  ),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: const Color(0xFFF59E0B),
                    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  InputDecoration _inputDecoration(String hint) {
    return InputDecoration(
      hintText: hint,
      hintStyle: const TextStyle(color: Color(0xFF64748B), fontSize: 13),
      filled: true,
      fillColor: const Color(0xFF1E293B),
      contentPadding: const EdgeInsets.symmetric(horizontal: 14, vertical: 12),
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(10), borderSide: const BorderSide(color: Color(0xFF334155))),
      enabledBorder: OutlineInputBorder(borderRadius: BorderRadius.circular(10), borderSide: const BorderSide(color: Color(0xFF334155))),
      focusedBorder: OutlineInputBorder(borderRadius: BorderRadius.circular(10), borderSide: const BorderSide(color: Color(0xFFF59E0B))),
    );
  }
}
