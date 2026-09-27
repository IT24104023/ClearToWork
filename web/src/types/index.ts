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

export interface Contractor {
  id: string;
  name: string;
  code: string;
  isApproved: boolean;
}

export interface CertificateType {
  id: string;
  code: string;
  name: string;
  validityMonths: number;
}

export interface CreateWorkerRequest {
  firstName: string;
  lastName: string;
  badgeNumber: string;
  trade: string;
  contractorId: string;
}

export interface UpdateWorkerRequest {
  firstName: string;
  lastName: string;
  trade: string;
  isActive: boolean;
}

export interface CreateCertificateRequest {
  certificateTypeId: string;
  certificateNumber: string;
  issuingBody: string;
  issueDate: string;
  expiryDate: string;
}

export interface CreateAssetRequest {
  assetTag: string;
  name: string;
  category: string;
  currentZoneId?: string;
}

export interface UpdateAssetRequest {
  name: string;
  category: string;
  status: string;
  currentZoneId?: string;
}

export interface CreateInspectionRequest {
  inspectorName: string;
  isPassed: boolean;
  notes?: string;
  nextInspectionDate?: string;
}

export interface CreateCalibrationRequest {
  calibratedBy: string;
  certificateNumber: string;
  isPassed: boolean;
  calibrationDate?: string;
  nextCalibrationDate?: string;
}

export interface IsolationPoint {
  id: string;
  code: string;
  name: string;
  zoneId: string;
  zoneName?: string;
  currentState: 'Open' | 'LockedOut' | 'TaggedOut';
  lastInspectedAt?: string;
  notes?: string;
}

export interface CreateIsolationPointRequest {
  code: string;
  name: string;
  zoneId: string;
  notes?: string;
}

export interface UpdateIsolationPointStateRequest {
  state: 'Open' | 'LockedOut' | 'TaggedOut';
  notes?: string;
}

export interface UpdatePermitRequest {
  permitTypeId: string;
  zoneId: string;
  objectiveDescription: string;
  scheduledStartTime: string;
  scheduledEndTime: string;
  workerIds: string[];
  assetIds: string[];
  photoUrls?: string[];
}

export interface Observation {
  id: string;
  zoneId: string;
  zoneCode: string;
  zoneName: string;
  reportedByUserId: string;
  reporterName: string;
  category: 'NearMiss' | 'Hazard' | 'UnsafeAct' | 'UnsafeCondition' | 'Positive';
  description: string;
  loggedAt: string;
  createdAt: string;
}

export interface CreateObservationRequest {
  zoneId: string;
  category: string;
  description: string;
}

export interface UpdateObservationRequest {
  zoneId?: string;
  category: string;
  description: string;
}

export interface ControlMeasure {
  id: string;
  hazardTypeId: string;
  code: string;
  requirementDescription: string;
  isMandatory: boolean;
}

export interface HazardType {
  id: string;
  code: string;
  name: string;
  severityLevel: 'Low' | 'Medium' | 'High' | 'Critical';
  maxWindSpeedKmh?: number | null;
  prohibitedInRain: boolean;
  controlMeasures: ControlMeasure[];
}

export interface CreateHazardTypeRequest {
  code: string;
  name: string;
  severityLevel: string;
  maxWindSpeedKmh?: number | null;
  prohibitedInRain: boolean;
}

export interface UpdateHazardTypeRequest {
  name: string;
  severityLevel: string;
  maxWindSpeedKmh?: number | null;
  prohibitedInRain: boolean;
}

export interface CreateControlMeasureRequest {
  hazardTypeId: string;
  code: string;
  requirementDescription: string;
  isMandatory: boolean;
}

export interface UpdateControlMeasureRequest {
  requirementDescription: string;
  isMandatory: boolean;
}

export interface IncompatibilityRule {
  id: string;
  ruleCode: string;
  primaryHazardId: string;
  primaryHazardCode: string;
  primaryHazardName: string;
  conflictingHazardId: string;
  conflictingHazardCode: string;
  conflictingHazardName: string;
  reason: string;
  appliesToAdjacentZones: boolean;
}

export interface CreateIncompatibilityRuleRequest {
  ruleCode: string;
  primaryHazardId: string;
  conflictingHazardId: string;
  reason: string;
  appliesToAdjacentZones: boolean;
}

export interface UpdateIncompatibilityRuleRequest {
  reason: string;
  appliesToAdjacentZones: boolean;
}

export interface ZoneAdjacency {
  zoneId: string;
  zoneCode: string;
  zoneName: string;
  adjacentZoneId: string;
  adjacentZoneCode: string;
  adjacentZoneName: string;
}

export interface AddZoneAdjacencyRequest {
  zoneId: string;
  adjacentZoneId: string;
}

export interface SafetyAnalytics {
  totalPermitsIssued: number;
  totalPermitsRefused: number;
  activePermitsCount: number;
  meanTimeToApprovalHours: number;
  refusalCausesRanking: Record<string, number>;
  permitsByZoneDistribution: Record<string, number>;
}


