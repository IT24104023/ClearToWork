namespace ClearToWork.Domain.Enums;

public enum UserRole
{
    ContractorSupervisor = 1,
    AreaSupervisor = 2,
    SafetyOfficer = 3,
    Administrator = 4
}

public enum PermitStatus
{
    Draft = 1,
    Submitted = 2,
    AiReview = 3,
    PendingApproval = 4,
    Approved = 5,
    Active = 6,
    Closed = 7,
    Refused = 8,
    Expired = 9
}

public enum CertificateStatus
{
    Valid = 1,
    Expiring = 2,
    Expired = 3,
    Suspended = 4
}

public enum AssetStatus
{
    Available = 1,
    Reserved = 2,
    InUse = 3,
    OutOfService = 4
}

public enum AssetCategory
{
    GasDetector = 1,
    Extinguisher = 2,
    Harness = 3,
    WeldingSet = 4,
    Scaffold = 5,
    VentilationBlower = 6
}

public enum IsolationType
{
    Electrical = 1,
    Valve = 2,
    Mechanical = 3
}

public enum IsolationState
{
    Open = 1,
    LockedOut = 2,
    TaggedOut = 3
}

public enum WorkflowOutcome
{
    Clear = 1,
    Refused_SafeFailure = 2,
    NeedsRevision = 3,
    Error = 4
}

public enum DecisionType
{
    Approved = 1,
    Rejected = 2,
    RevisionRequested = 3
}

public enum HazardSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

