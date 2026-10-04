import 'package:flutter/material.dart';
import '../models/permit_model.dart';
import '../services/api_service.dart';
import '../widgets/status_badge_widget.dart';
import 'permit_detail_screen.dart';
import 'create_permit_screen.dart';

/// Permits List Screen displaying active permit requests consumed from the shared ASP.NET Core API.
class PermitListScreen extends StatefulWidget {
  const PermitListScreen({Key? key}) : super(key: key);

  @override
  State<PermitListScreen> createState() => _PermitListScreenState();
}

class _PermitListScreenState extends State<PermitListScreen> {
  late Future<List<PermitModel>> _permitsFuture;
  String _selectedFilter = 'All';

  @override
  void initState() {
    super.initState();
    _loadPermits();
  }

  void _loadPermits() {
    setState(() {
      _permitsFuture = ApiService.getPermits();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFF0F172A),
      appBar: AppBar(
        backgroundColor: const Color(0xFF1E293B),
        title: const Text('Safety Permits Register', style: TextStyle(color: Colors.white, fontSize: 16, fontWeight: FontWeight.bold)),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh, color: Color(0xFFF59E0B)),
            onPressed: _loadPermits,
          )
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        backgroundColor: const Color(0xFFF59E0B),
        icon: const Icon(Icons.add, color: Color(0xFF0F172A)),
        label: const Text('New Permit', style: TextStyle(color: Color(0xFF0F172A), fontWeight: FontWeight.bold)),
        onPressed: () async {
          final created = await Navigator.of(context).push(
            MaterialPageRoute(builder: (_) => const CreatePermitScreen()),
          );
          if (created == true) {
            _loadPermits();
          }
        },
      ),
      body: Column(
        children: [
          // Filter Tabs
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
            color: const Color(0xFF1E293B),
            child: SingleChildScrollView(
              scrollDirection: Axis.horizontal,
              child: Row(
                children: ['All', 'Active', 'Approved', 'Refused', 'Draft'].map((filter) {
                  final isSelected = _selectedFilter == filter;
                  return Padding(
                    padding: const EdgeInsets.only(right: 8.0),
                    child: FilterChip(
                      selected: isSelected,
                      label: Text(filter, style: TextStyle(color: isSelected ? const Color(0xFF0F172A) : Colors.white, fontSize: 12, fontWeight: FontWeight.bold)),
                      selectedColor: const Color(0xFFF59E0B),
                      backgroundColor: const Color(0xFF334155),
                      onSelected: (val) {
                        setState(() {
                          _selectedFilter = filter;
                        });
                      },
                    ),
                  );
                }).toList(),
              ),
            ),
          ),

          // FutureBuilder to load permits from ASP.NET Core API
          Expanded(
            child: FutureBuilder<List<PermitModel>>(
              future: _permitsFuture,
              builder: (context, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting) {
                  return const Center(child: CircularProgressIndicator(color: Color(0xFFF59E0B)));
                }

                if (snapshot.hasError) {
                  return Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(Icons.error_outline, color: Color(0xFFEF4444), size: 48),
                        const SizedBox(height: 12),
                        const Text('Failed to load permits from ASP.NET Core backend.', style: TextStyle(color: Colors.white)),
                        const SizedBox(height: 12),
                        ElevatedButton(onPressed: _loadPermits, child: const Text('Retry Connection'))
                      ],
                    ),
                  );
                }

                final permits = snapshot.data ?? [];
                final filtered = permits.where((p) {
                  if (_selectedFilter == 'All') return true;
                  return p.status.toLowerCase() == _selectedFilter.toLowerCase();
                }).toList();

                if (filtered.isEmpty) {
                  return const Center(
                    child: Text('No permits found matching selected filter.', style: TextStyle(color: Color(0xFF94A3B8))),
                  );
                }

                return ListView.builder(
                  padding: const EdgeInsets.all(16),
                  itemCount: filtered.length,
                  itemBuilder: (context, index) {
                    final permit = filtered[index];
                    return Card(
                      color: const Color(0xFF1E293B),
                      margin: const EdgeInsets.only(bottom: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                        side: const BorderSide(color: Color(0xFF334155)),
                      ),
                      child: InkWell(
                        onTap: () {
                          Navigator.of(context).push(
                            MaterialPageRoute(builder: (_) => PermitDetailScreen(permit: permit)),
                          );
                        },
                        borderRadius: BorderRadius.circular(12),
                        child: Padding(
                          padding: const EdgeInsets.all(16.0),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  Text(
                                    permit.permitNumber,
                                    style: const TextStyle(color: Color(0xFFF59E0B), fontWeight: FontWeight.bold, fontSize: 13, fontFamily: 'monospace'),
                                  ),
                                  StatusBadgeWidget(status: permit.status),
                                ],
                              ),
                              const SizedBox(height: 8),
                              Text(permit.title, style: const TextStyle(color: Colors.white, fontWeight: FontWeight.bold, fontSize: 15)),
                              const SizedBox(height: 4),
                              Text(permit.objectiveDescription, style: const TextStyle(color: Color(0xFF94A3B8), fontSize: 12), maxLines: 2, overflow: TextOverflow.ellipsis),
                              const SizedBox(height: 12),
                              Row(
                                children: [
                                  const Icon(Icons.location_on_outlined, size: 14, color: Color(0xFF94A3B8)),
                                  const SizedBox(width: 4),
                                  Text(permit.zoneCode, style: const TextStyle(color: Color(0xFFCBD5E1), fontSize: 11)),
                                  const SizedBox(width: 16),
                                  const Icon(Icons.person_outline, size: 14, color: Color(0xFF94A3B8)),
                                  const SizedBox(width: 4),
                                  Text(permit.supervisorName, style: const TextStyle(color: Color(0xFFCBD5E1), fontSize: 11)),
                                ],
                              )
                            ],
                          ),
                        ),
                      ),
                    );
                  },
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
