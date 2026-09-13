export type UserRole = 'ContractorSupervisor' | 'AreaSupervisor' | 'SafetyOfficer' | 'Administrator';

export interface UserSession {
  id: string;
  fullName: string;
  email: string;
  role: UserRole;
  contractorId?: string | null;
  token?: string;
}

export type PermitStatus = 'Draft' | 'Submitted' | 'AiReview' | 'PendingApproval' | 'Approved' | 'Active' | 'Closed' | 'Refused';

export interface EvidencePhoto {
  id: string;
  stage: string;
  photoUrl: string;
  gpsLatitude?: number;
  gpsLongitude?: number;
  capturedAt: string;
}

export interface WorkerCertificate {
  id: string;
  certificateCode: string;
  certificateName: string;
  certificateNumber: string;
  issuingBody: string;
  issueDate: string;
  expiryDate: string;
  status: string;
  daysUntilExpiry: number;
}

export interface Worker {
  id: string;
  badgeNumber: string;
  firstName: string;
  lastName: string;
  trade: string;
  contractorId: string;
  contractorName: string;
  isActive: boolean;
  certificates: WorkerCertificate[];
}

export interface Asset {
  id: string;
  assetTag: string;
  name: string;
  category: string;
  status: string;
  currentZoneId?: string;
  isCalibrationValid: boolean;
  nextCalibrationDate?: string;
  isInspectionValid: boolean;
  nextInspectionDate?: string;
}

export interface Approval {
  safetyOfficerName: string;
  decision: 'Approved' | 'Rejected' | 'RevisionRequested';
  decisionNotes: string;
  decisionTimestamp: string;
}

export interface ToolTrace {
  tool_name: string;
  arguments: Record<string, any>;
  result: any;
  latency_ms: number;
  status: string;
}

export interface AgentStepTrace {
  agent: string;
  owner: string;
  tool?: string;
  tools_called?: ToolTrace[];
  findings?: string[];
  verdict?: string;
  latency_ms: number;
  status: string;
}

export interface AgentWorkflowRun {
  id: string;
  outcomeStatus: string;
  durationMs: number;
  modelUsed: string;
  executionTraceJson: string;
  recommendedFixJson?: string;
  createdAt: string;
}

export interface PermitDetails {
  id: string;
  permitNumber: string;
  permitTypeName: string;
  permitTypeCode: string;
  zoneName: string;
  zoneCode: string;
  supervisorName: string;
  objectiveDescription: string;
  scheduledStartTime: string;
  scheduledEndTime: string;
  status: PermitStatus;
  permitQrToken?: string;
  activatedAt?: string;
  assignedWorkers: Worker[];
  assignedAssets: Asset[];
  photos: EvidencePhoto[];
  approval?: Approval;
  workflowRun?: AgentWorkflowRun;
}

export interface ProposedFix {
  suggestedWorkerBadge?: string;
  suggestedAssetTag?: string;
  suggestedTimeWindow?: string;
  summaryExplanation?: string;
}

export interface ValidationReport {
  isApproved: boolean;
  verdict: string;
  hardFailureReasons: string[];
  warningNotes: string[];
  proposedFix?: ProposedFix;
}

export interface Zone {
  id: string;
  siteId: string;
  code: string;
  name: string;
  latitude: number;
  longitude: number;
  radiusMeters: number;
  qrCodePayload: string;
  isActive: boolean;
  adjacentZoneCodes: string[];
}

export interface SafetyAnalytics {
  totalPermitsIssued: number;
  totalPermitsRefused: number;
  activePermitsCount: number;
  meanTimeToApprovalHours: number;
  refusalCausesRanking: Record<string, number>;
  permitsByZoneDistribution: Record<string, number>;
}
