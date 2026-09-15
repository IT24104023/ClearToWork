# ClearToWork AI — Database Schema & Entity Relational Design

## 1. Relational Design Principles
The ClearToWork AI database model is designed using Entity Framework Core 8 with strict relational integrity:
- **Primary Keys:** Globally unique UUIDs (`Guid`).
- **Audit Columns:** Every domain entity inherits `BaseAuditableEntity` (`CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`).
- **Enums as Strings:** All domain enums (roles, statuses, severity) serialize cleanly across SQL and JSON.
- **Relational Integrity:** Foreign keys are explicitly defined with cascading or restricted delete behaviors to prevent orphaned safety records.

---

## 2. Core Relational Domains

### 2.1 Identity Domain
- **`Users`**: `Id`, `FullName`, `Email`, `PasswordHash`, `Role`, `ContractorId`, `AvatarUrl`, `PhoneNumber`, `Department`, `Bio`, `IsActive`, `CreatedAt`, `UpdatedAt`.

### 2.2 Workforce Domain (Student 1)
- **`Contractors`**: `Id`, `Name`, `CompanyRegistrationNumber`, `ContactEmail`, `SafetyRating`, `IsApproved`.
- **`Workers`**: `Id`, `BadgeNumber`, `FirstName`, `LastName`, `Trade`, `ContractorId`, `IsActive`.
- **`CertificateTypes`**: `Id`, `Code`, `Name`, `Trade`, `ValidityPeriodMonths`, `RequiresPracticalExam`.
- **`WorkerCertificates`**: `Id`, `WorkerId`, `CertificateTypeId`, `CertificateNumber`, `IssuingBody`, `IssueDate`, `ExpiryDate`, `Status`.

### 2.3 Equipment & Isolation Domain (Student 2)
- **`Assets`**: `Id`, `AssetTag`, `Name`, `Category`, `Status`, `CurrentZoneId`, `SerialNumber`.
- **`InspectionRecords`**: `Id`, `AssetId`, `InspectionDate`, `NextInspectionDate`, `InspectorName`, `Passed`, `Notes`.
- **`CalibrationRecords`**: `Id`, `AssetId`, `CalibrationDate`, `NextCalibrationDate`, `CalibratedBy`, `CertificateNumber`, `PassStatus`.
- **`IsolationPoints`**: `Id`, `ZoneId`, `Tag`, `Description`, `IsolationType`, `Status`.

### 2.4 Permit Lifecycle Domain (Student 3)
- **`PermitTypes`**: `Id`, `Code`, `Name`, `MaxDurationHours`, `RequiresFireWatch`, `RequiresGasTesting`, `MandatoryControlsJson`.
- **`PermitRequests`**: `Id`, `PermitNumber`, `PermitTypeId`, `ZoneId`, `SupervisorId`, `ObjectiveDescription`, `ScheduledStartTime`, `ScheduledEndTime`, `Status`, `PermitQrToken`.
- **`PermitWorkers`**: `PermitRequestId`, `WorkerId`, `RoleOnPermit`.
- **`PermitAssets`**: `PermitRequestId`, `AssetId`, `ReservedFrom`, `ReservedUntil`.
- **`Approvals`**: `Id`, `PermitRequestId`, `SafetyOfficerId`, `Decision`, `DecisionNotes`, `DecisionTimestamp`.
- **`EvidencePhotos`**: `Id`, `PermitRequestId`, `Stage`, `PhotoUrl`, `CapturedAt`.

### 2.5 Hazard Zones & SIMOPS Domain (Student 4)
- **`Sites`**: `Id`, `Name`, `Code`, `Latitude`, `Longitude`.
- **`Zones`**: `Id`, `SiteId`, `Code`, `Name`, `Latitude`, `Longitude`, `RadiusMeters`, `QrCodePayload`, `IsActive`.
- **`ZoneAdjacencies`**: `Id`, `ZoneId`, `AdjacentZoneId`, `RiskTransferLevel`.
- **`HazardTypes`**: `Id`, `Code`, `Name`, `SeverityLevel`, `MaxWindSpeedKmh`, `ProhibitedInRain`.
- **`HazardRules`**: `Id`, `RuleCode`, `PrimaryHazardId`, `ConflictingHazardId`, `IsForbiddenSimultaneously`, `Explanation`.

### 2.6 Agentic AI Observability Domain (Shared)
- **`AgentWorkflowRuns`**: `Id`, `PermitRequestId`, `OutcomeStatus`, `DurationMs`, `ModelUsed`, `ExecutionTraceJson`, `RecommendedFixJson`, `CreatedAt`.
