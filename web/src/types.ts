export type PermitStatus = 'Draft' | 'Submitted' | 'UnderReview' | 'Approved' | 'Active' | 'Suspended' | 'Closed' | 'Rejected' | 'Archived';

export interface PermitRequest {
  id: string;
  permitNumber: string;
  title: string;
  description: string;
  status: PermitStatus;
  zoneCode: string;
  issuingAuthority: string;
  supervisorName: string;
  scheduledStartTime: string;
  scheduledEndTime: string;
}

export interface Worker {
  id: string;
  badgeNumber: string;
  firstName: string;
  lastName: string;
  trade: string;
  companyName: string;
  isActive: boolean;
}

export interface Asset {
  id: string;
  assetTag: string;
  serialNumber: string;
  model: string;
  category: string;
  status: string;
}

export interface HazardType {
  id: string;
  code: string;
  name: string;
  severity: string;
}

export interface IncompatibilityRule {
  id: string;
  primaryHazard: string;
  secondaryHazard: string;
  actionRequired: string;
  isProhibited: boolean;
}
