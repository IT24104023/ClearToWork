import 'dart:convert';

/// Represents a Permit-to-Work request entity consumed from the ASP.NET Core REST API.
class PermitModel {
  final String id;
  final String permitNumber;
  final String title;
  final String objectiveDescription;
  final String status;
  final String zoneCode;
  final String issuingAuthority;
  final String supervisorName;
  final DateTime scheduledStartTime;
  final DateTime scheduledEndTime;
  final String? agentTraceJson;
  final String? recommendedFixJson;
  final List<String> assignedWorkerNames;
  final List<String> assignedAssetSerialNumbers;

  PermitModel({
    required this.id,
    required this.permitNumber,
    required this.title,
    required this.objectiveDescription,
    required this.status,
    required this.zoneCode,
    required this.issuingAuthority,
    required this.supervisorName,
    required this.scheduledStartTime,
    required this.scheduledEndTime,
    this.agentTraceJson,
    this.recommendedFixJson,
    required this.assignedWorkerNames,
    required this.assignedAssetSerialNumbers,
  });

  factory PermitModel.fromJson(Map<String, dynamic> json) {
    List<String> workers = [];
    if (json['assignedWorkers'] != null && json['assignedWorkers'] is List) {
      workers = (json['assignedWorkers'] as List)
          .map((w) => (w['fullName'] ?? w['name'] ?? 'Worker').toString())
          .toList();
    }

    List<String> assets = [];
    if (json['assignedAssets'] != null && json['assignedAssets'] is List) {
      assets = (json['assignedAssets'] as List)
          .map((a) => (a['serialNumber'] ?? a['assetType'] ?? 'Asset').toString())
          .toList();
    }

    return PermitModel(
      id: json['id'] ?? '',
      permitNumber: json['permitNumber'] ?? 'PTW-2026',
      title: json['title'] ?? 'Industrial Permit',
      objectiveDescription: json['objectiveDescription'] ?? json['description'] ?? '',
      status: json['status'] ?? 'Draft',
      zoneCode: json['zoneCode'] ?? 'ZONE-A1',
      issuingAuthority: json['issuingAuthority'] ?? 'HSE Safety Officer',
      supervisorName: json['supervisorName'] ?? 'Contractor Supervisor',
      scheduledStartTime: json['scheduledStartTime'] != null
          ? DateTime.parse(json['scheduledStartTime'])
          : DateTime.now(),
      scheduledEndTime: json['scheduledEndTime'] != null
          ? DateTime.parse(json['scheduledEndTime'])
          : DateTime.now().add(const Duration(hours: 8)),
      agentTraceJson: json['agentTraceJson'],
      recommendedFixJson: json['recommendedFixJson'],
      assignedWorkerNames: workers,
      assignedAssetSerialNumbers: assets,
    );
  }
}
