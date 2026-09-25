using System.Text.RegularExpressions;
using Xunit;

namespace ClearToWork.Tests.Equipment;

#region Equipment & LOTO Domain Models & Enums

/// <summary>
/// Categories of safety equipment, gas detection monitors, and life-critical tools.
/// </summary>
public enum EquipmentType
{
    GasDetector = 1,
    BreathingApparatus_SCBA = 2,
    LotoPadlock = 3,
    ConfinedSpaceTripod = 4,
    HarnessLanyard = 5,
    VoltageDetector = 6,
    VentilationBlower = 7
}

/// <summary>
/// Operational status of safety equipment in the ClearToWork tool crib.
/// </summary>
public enum EquipmentStatus
{
    Available = 1,
    CheckedOut = 2,
    InMaintenance = 3,
    Quarantined = 4,
    Decommissioned = 5
}

/// <summary>
/// Energy sources isolated under Lockout/Tagout (LOTO) protocols.
/// </summary>
public enum IsolationType
{
    Electrical = 1,
    MechanicalValve = 2,
    Hydraulic = 3,
    Pneumatic = 4,
    ChemicalBlind = 5
}

/// <summary>
/// Lockout/Tagout state progression for physical isolation points.
/// </summary>
public enum IsolationState
{
    OpenDeIsolated = 1,
    LockedIsolated = 2,
    TaggedOut = 3,
    VerifiedZeroEnergy = 4
}

/// <summary>
/// Domain entity representing a tracked physical asset or safety instrument.
/// </summary>
public class EquipmentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SerialNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public EquipmentType Type { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Available;
    public DateTime? LastCalibrationDate { get; set; }
    public DateTime? NextCalibrationDueDate { get; set; }
    public double OperatingHours { get; set; }
    public bool RequiresCalibration { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CurrentAssignedPermit { get; set; }
    public Guid? CurrentAssignedWorkerId { get; set; }
}

/// <summary>
/// Immutable audit record of a formal calibration performed by an instrument technician.
/// </summary>
public record CalibrationRecord(
    Guid Id,
    Guid EquipmentId,
    DateTime CalibrationDate,
    DateTime NextDueDate,
    string TechnicianBadge,
    string CertificateNumber,
    bool Passed,
    string SpanGasBatchNumber,
    string? Notes
);

/// <summary>
/// Physical energy isolation point (breaker, ball valve, spectacle blind).
/// </summary>
public class IsolationPoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TagNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ZoneCode { get; set; } = string.Empty;
    public IsolationType Type { get; set; }
    public IsolationState State { get; set; } = IsolationState.OpenDeIsolated;
    public Guid? ActivePermitId { get; set; }
    public List<LotoLock> AppliedLocks { get; set; } = new();
    public bool ZeroEnergyVerified { get; set; }
    public string? ZeroEnergyVerifiedBy { get; set; }
    public DateTime? ZeroEnergyVerifiedAt { get; set; }
}

/// <summary>
/// Physical padlock applied to an isolation point hasp.
/// </summary>
public record LotoLock(
    Guid LockId,
    string LockSerialNumber,
    Guid WorkerId,
    string WorkerName,
    DateTime AppliedAtUtc
);

/// <summary>
/// Checkout checkout and checkin audit log entry.
/// </summary>
public class EquipmentCheckoutRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EquipmentId { get; set; }
    public Guid WorkerId { get; set; }
    public string PermitNumber { get; set; } = string.Empty;
    public DateTime CheckoutTimeUtc { get; set; }
    public DateTime? CheckinTimeUtc { get; set; }
    public double HoursLogged { get; set; }
    public string? ReturnConditionNotes { get; set; }
    public bool IsReturned => CheckinTimeUtc.HasValue;
}

#endregion

#region Domain Exceptions

public class InvalidSerialNumberException : Exception
{
    public InvalidSerialNumberException(string message) : base(message) { }
}

public class EquipmentOverdueException : Exception
{
    public EquipmentOverdueException(string message) : base(message) { }
}

public class EquipmentUnavailableException : Exception
{
    public EquipmentUnavailableException(string message) : base(message) { }
}

public class LotoViolationException : Exception
{
    public LotoViolationException(string message) : base(message) { }
}

#endregion

#region Service Contract & Implementation

/// <summary>
/// Interface managing safety equipment lifecycle, calibration certification,
/// LOTO isolation tracking, and equipment custody checkout/checkin.
/// </summary>
public interface IEquipmentService
{
    EquipmentItem RegisterEquipment(string serialNumber, string model, EquipmentType type, bool requiresCalibration, DateTime? initialCalibrationDate = null);
    CalibrationRecord RecordCalibration(Guid equipmentId, string technicianBadge, string certificateNumber, bool passed, string spanGasBatch, TimeSpan validDuration, string? notes = null);
    IReadOnlyList<EquipmentItem> GetOverdueEquipment(DateTime? asOfUtc = null);
    IsolationPoint RegisterIsolationPoint(string tagNumber, string description, string zoneCode, IsolationType type);
    void ApplyLockout(Guid isolationPointId, Guid permitId, Guid workerId, string workerName, string lockSerialNumber);
    void VerifyZeroEnergy(Guid isolationPointId, string verifiedByBadge);
    void RemoveLockout(Guid isolationPointId, Guid workerId, string lockSerialNumber);
    EquipmentCheckoutRecord CheckoutEquipment(Guid equipmentId, Guid workerId, string permitNumber, DateTime? asOfUtc = null);
    EquipmentCheckoutRecord CheckinEquipment(Guid checkoutRecordId, double operatingHoursAdded, string returnCondition, DateTime? asOfUtc = null);
}

/// <summary>
/// Production service implementing safety equipment management and LOTO custody for ClearToWork AI.
/// </summary>
public class EquipmentService : IEquipmentService
{
    private readonly Dictionary<Guid, EquipmentItem> _equipment = new();
    private readonly List<CalibrationRecord> _calibrationHistory = new();
    private readonly Dictionary<Guid, IsolationPoint> _isolationPoints = new();
    private readonly Dictionary<Guid, EquipmentCheckoutRecord> _checkouts = new();

    // Serial format: 3-12 alphanumeric uppercase characters with optional hyphens (e.g., "BW-ULTRA-001", "VENTIS-9821")
    private static readonly Regex SerialNumberPattern = new(@"^[A-Z0-9]{2,6}(-[A-Z0-9]{2,8})+$", RegexOptions.Compiled);

    /// <inheritdoc />
    public EquipmentItem RegisterEquipment(string serialNumber, string model, EquipmentType type, bool requiresCalibration, DateTime? initialCalibrationDate = null)
    {
        if (string.IsNullOrWhiteSpace(serialNumber) || !SerialNumberPattern.IsMatch(serialNumber.Trim().ToUpperInvariant()))
        {
            throw new InvalidSerialNumberException($"Serial number '{serialNumber}' is invalid. Must conform to standard asset tagging pattern (e.g., 'BW-ULTRA-001').");
        }

        var normalizedSerial = serialNumber.Trim().ToUpperInvariant();
        if (_equipment.Values.Any(e => e.SerialNumber.Equals(normalizedSerial, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidSerialNumberException($"An equipment asset with serial number '{normalizedSerial}' is already registered in the facility database.");
        }

        var item = new EquipmentItem
        {
            SerialNumber = normalizedSerial,
            Model = model,
            Type = type,
            RequiresCalibration = requiresCalibration,
            Status = EquipmentStatus.Available,
            LastCalibrationDate = initialCalibrationDate,
            NextCalibrationDueDate = initialCalibrationDate.HasValue ? initialCalibrationDate.Value.AddDays(30) : null
        };

        _equipment[item.Id] = item;
        return item;
    }

    /// <inheritdoc />
    public CalibrationRecord RecordCalibration(
        Guid equipmentId,
        string technicianBadge,
        string certificateNumber,
        bool passed,
        string spanGasBatch,
        TimeSpan validDuration,
        string? notes = null)
    {
        if (!_equipment.TryGetValue(equipmentId, out var item))
        {
            throw new KeyNotFoundException($"Equipment asset with ID '{equipmentId}' does not exist.");
        }

        var now = DateTime.UtcNow;
        var nextDueDate = now.Add(validDuration);

        var record = new CalibrationRecord(
            Id: Guid.NewGuid(),
            EquipmentId: equipmentId,
            CalibrationDate: now,
            NextDueDate: nextDueDate,
            TechnicianBadge: technicianBadge,
            CertificateNumber: certificateNumber,
            Passed: passed,
            SpanGasBatchNumber: spanGasBatch,
            Notes: notes
        );

        _calibrationHistory.Add(record);

        if (passed)
        {
            item.LastCalibrationDate = now;
            item.NextCalibrationDueDate = nextDueDate;
            item.OperatingHours = 0; // Reset sensor cycle on full calibration
            if (item.Status == EquipmentStatus.Quarantined)
            {
                item.Status = EquipmentStatus.Available;
            }
        }
        else
        {
            // Failed calibration locks out the device
            item.Status = EquipmentStatus.Quarantined;
        }

        return record;
    }

    /// <inheritdoc />
    public IReadOnlyList<EquipmentItem> GetOverdueEquipment(DateTime? asOfUtc = null)
    {
        var now = asOfUtc ?? DateTime.UtcNow;

        return _equipment.Values
            .Where(e => e.RequiresCalibration &&
                        e.Status != EquipmentStatus.Decommissioned &&
                        (!e.NextCalibrationDueDate.HasValue || e.NextCalibrationDueDate.Value < now))
            .ToList();
    }

    /// <inheritdoc />
    public IsolationPoint RegisterIsolationPoint(string tagNumber, string description, string zoneCode, IsolationType type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagNumber);

        var point = new IsolationPoint
        {
            TagNumber = tagNumber.Trim().ToUpperInvariant(),
            Description = description,
            ZoneCode = zoneCode,
            Type = type,
            State = IsolationState.OpenDeIsolated
        };

        _isolationPoints[point.Id] = point;
        return point;
    }

    /// <inheritdoc />
    public void ApplyLockout(Guid isolationPointId, Guid permitId, Guid workerId, string workerName, string lockSerialNumber)
    {
        if (!_isolationPoints.TryGetValue(isolationPointId, out var point))
        {
            throw new KeyNotFoundException($"Isolation point '{isolationPointId}' not found.");
        }

        if (point.ActivePermitId.HasValue && point.ActivePermitId.Value != permitId)
        {
            throw new LotoViolationException($"Isolation point '{point.TagNumber}' is already locked under permit '{point.ActivePermitId.Value}'. Cross-permit lock clash.");
        }

        if (point.AppliedLocks.Any(l => l.LockSerialNumber == lockSerialNumber))
        {
            throw new LotoViolationException($"Lock with serial number '{lockSerialNumber}' is already applied to isolation point '{point.TagNumber}'.");
        }

        point.ActivePermitId = permitId;
        point.AppliedLocks.Add(new LotoLock(Guid.NewGuid(), lockSerialNumber, workerId, workerName, DateTime.UtcNow));
        point.State = IsolationState.LockedIsolated;
    }

    /// <inheritdoc />
    public void VerifyZeroEnergy(Guid isolationPointId, string verifiedByBadge)
    {
        if (!_isolationPoints.TryGetValue(isolationPointId, out var point))
        {
            throw new KeyNotFoundException($"Isolation point '{isolationPointId}' not found.");
        }

        if (point.State != IsolationState.LockedIsolated)
        {
            throw new LotoViolationException($"Cannot verify zero energy state on '{point.TagNumber}' because it is not physically locked (Current State: {point.State}).");
        }

        point.ZeroEnergyVerified = true;
        point.ZeroEnergyVerifiedBy = verifiedByBadge;
        point.ZeroEnergyVerifiedAt = DateTime.UtcNow;
        point.State = IsolationState.VerifiedZeroEnergy;
    }

    /// <inheritdoc />
    public void RemoveLockout(Guid isolationPointId, Guid workerId, string lockSerialNumber)
    {
        if (!_isolationPoints.TryGetValue(isolationPointId, out var point))
        {
            throw new KeyNotFoundException($"Isolation point '{isolationPointId}' not found.");
        }

        var lockToRemove = point.AppliedLocks.FirstOrDefault(l => l.LockSerialNumber == lockSerialNumber);
        if (lockToRemove == null)
        {
            throw new LotoViolationException($"Lock '{lockSerialNumber}' is not attached to isolation point '{point.TagNumber}'.");
        }

        // Rule of Personal Lock Ownership: Only the worker who applied the padlock can remove it
        if (lockToRemove.WorkerId != workerId)
        {
            throw new LotoViolationException($"Unauthorized lock removal attempt! Worker '{workerId}' cannot remove personal safety lock applied by '{lockToRemove.WorkerName}' ({lockToRemove.WorkerId}).");
        }

        point.AppliedLocks.Remove(lockToRemove);

        if (point.AppliedLocks.Count == 0)
        {
            point.ActivePermitId = null;
            point.ZeroEnergyVerified = false;
            point.ZeroEnergyVerifiedBy = null;
            point.ZeroEnergyVerifiedAt = null;
            point.State = IsolationState.OpenDeIsolated;
        }
    }

    /// <inheritdoc />
    public EquipmentCheckoutRecord CheckoutEquipment(Guid equipmentId, Guid workerId, string permitNumber, DateTime? asOfUtc = null)
    {
        var now = asOfUtc ?? DateTime.UtcNow;

        if (!_equipment.TryGetValue(equipmentId, out var item))
        {
            throw new KeyNotFoundException($"Equipment '{equipmentId}' does not exist.");
        }

        if (item.Status != EquipmentStatus.Available)
        {
            throw new EquipmentUnavailableException($"Equipment '{item.SerialNumber}' cannot be checked out. Current status: '{item.Status}'.");
        }

        if (item.RequiresCalibration)
        {
            if (!item.NextCalibrationDueDate.HasValue || item.NextCalibrationDueDate.Value < now)
            {
                throw new EquipmentOverdueException($"Safety Lockout: Cannot check out '{item.SerialNumber}'. Calibration is expired or missing.");
            }
        }

        item.Status = EquipmentStatus.CheckedOut;
        item.CurrentAssignedPermit = permitNumber;
        item.CurrentAssignedWorkerId = workerId;

        var checkoutRecord = new EquipmentCheckoutRecord
        {
            EquipmentId = equipmentId,
            WorkerId = workerId,
            PermitNumber = permitNumber,
            CheckoutTimeUtc = now
        };

        _checkouts[checkoutRecord.Id] = checkoutRecord;
        return checkoutRecord;
    }

    /// <inheritdoc />
    public EquipmentCheckoutRecord CheckinEquipment(Guid checkoutRecordId, double operatingHoursAdded, string returnCondition, DateTime? asOfUtc = null)
    {
        var now = asOfUtc ?? DateTime.UtcNow;

        if (!_checkouts.TryGetValue(checkoutRecordId, out var record))
        {
            throw new KeyNotFoundException($"Checkout record '{checkoutRecordId}' not found.");
        }

        if (record.IsReturned)
        {
            throw new InvalidOperationException($"Equipment for checkout '{checkoutRecordId}' was already checked in.");
        }

        if (!_equipment.TryGetValue(record.EquipmentId, out var item))
        {
            throw new KeyNotFoundException($"Equipment '{record.EquipmentId}' not found.");
        }

        record.CheckinTimeUtc = now;
        record.HoursLogged = operatingHoursAdded;
        record.ReturnConditionNotes = returnCondition;

        item.OperatingHours += Math.Max(0.0, operatingHoursAdded);
        item.CurrentAssignedPermit = null;
        item.CurrentAssignedWorkerId = null;

        if (returnCondition.Contains("Damaged", StringComparison.OrdinalIgnoreCase) ||
            returnCondition.Contains("Faulty", StringComparison.OrdinalIgnoreCase))
        {
            item.Status = EquipmentStatus.InMaintenance;
        }
        else
        {
            item.Status = EquipmentStatus.Available;
        }

        return record;
    }
}

#endregion

/// <summary>
/// Unit test suite verifying safety equipment registration, serial validation, calibration audit trails,
/// Lockout/Tagout isolation point integrity, and custody checkout/checkin workflows.
/// </summary>
public class EquipmentServiceTests
{
    private readonly EquipmentService _equipmentService;

    public EquipmentServiceTests()
    {
        _equipmentService = new EquipmentService();
    }

    [Fact]
    public void RegisterEquipment_WithValidAlphaNumericSerialNumber_SucceedsAndSetsInitialStatus()
    {
        // Arrange
        const string serial = "BW-ULTRA-1049";
        const string model = "Honeywell BW Ultra 5-Gas";

        // Act
        var item = _equipmentService.RegisterEquipment(
            serialNumber: serial,
            model: model,
            type: EquipmentType.GasDetector,
            requiresCalibration: true,
            initialCalibrationDate: DateTime.UtcNow
        );

        // Assert
        Assert.NotNull(item);
        Assert.Equal(serial, item.SerialNumber);
        Assert.Equal(model, item.Model);
        Assert.Equal(EquipmentType.GasDetector, item.Type);
        Assert.Equal(EquipmentStatus.Available, item.Status);
        Assert.True(item.RequiresCalibration);
        Assert.NotNull(item.NextCalibrationDueDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("INVALID SERIAL WITH SPACES")]
    [InlineData("$$$$$###")]
    [InlineData("A")]
    public void RegisterEquipment_WithInvalidSerialNumber_ThrowsInvalidSerialNumberException(string invalidSerial)
    {
        // Act & Assert
        Assert.Throws<InvalidSerialNumberException>(() =>
        {
            _equipmentService.RegisterEquipment(
                serialNumber: invalidSerial,
                model: "Industrial Scientific Ventis Pro5",
                type: EquipmentType.GasDetector,
                requiresCalibration: true
            );
        });
    }

    [Fact]
    public void RegisterEquipment_WithDuplicateSerialNumber_ThrowsInvalidSerialNumberException()
    {
        // Arrange
        const string serial = "MSA-ALTAIR-4XR";
        _equipmentService.RegisterEquipment(serial, "MSA Altair 4XR", EquipmentType.GasDetector, true);

        // Act & Assert
        var ex = Assert.Throws<InvalidSerialNumberException>(() =>
        {
            _equipmentService.RegisterEquipment(serial, "Duplicate Unit", EquipmentType.GasDetector, true);
        });

        Assert.Contains("already registered", ex.Message);
    }

    [Fact]
    public void RecordCalibration_WhenSuccessful_UpdatesEquipmentTimestampsAndCreatesAuditRecord()
    {
        // Arrange
        var item = _equipmentService.RegisterEquipment("BW-SOLO-H2S-01", "Honeywell Solo H2S", EquipmentType.GasDetector, true);
        const string technicianBadge = "TECH-4019";
        const string certNumber = "CAL-2026-SEP-0089";
        const string spanGasBatch = "CYL-H2S-25PPM-LOT992";

        // Act
        var record = _equipmentService.RecordCalibration(
            equipmentId: item.Id,
            technicianBadge: technicianBadge,
            certificateNumber: certNumber,
            passed: true,
            spanGasBatch: spanGasBatch,
            validDuration: TimeSpan.FromDays(7),
            notes: "Zero calibration and 25ppm span gas bump test verified within +/- 2% tolerance."
        );

        // Assert
        Assert.NotNull(record);
        Assert.True(record.Passed);
        Assert.Equal(certNumber, record.CertificateNumber);
        Assert.Equal(technicianBadge, record.TechnicianBadge);
        Assert.NotNull(item.LastCalibrationDate);
        Assert.NotNull(item.NextCalibrationDueDate);
        Assert.Equal(EquipmentStatus.Available, item.Status);
        Assert.True(item.NextCalibrationDueDate > item.LastCalibrationDate);
    }

    [Fact]
    public void GetOverdueEquipment_IdentifiesAssetsBeyondCalibrationDueDate()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-45);
        var item1 = _equipmentService.RegisterEquipment("DRA-XAM-2500-A", "Drager X-am 2500", EquipmentType.GasDetector, true, pastDate);
        var item2 = _equipmentService.RegisterEquipment("DRA-XAM-2500-B", "Drager X-am 2500", EquipmentType.GasDetector, true, DateTime.UtcNow);

        // Act
        var overdueList = _equipmentService.GetOverdueEquipment();

        // Assert
        Assert.Contains(overdueList, e => e.Id == item1.Id);
        Assert.DoesNotContain(overdueList, e => e.Id == item2.Id);
    }

    [Fact]
    public void CheckoutEquipment_WhenEquipmentIsCertifiedAndAvailable_SucceedsAndMarksCheckedOut()
    {
        // Arrange
        var item = _equipmentService.RegisterEquipment("SCOTT-SCBA-04", "Scott Safety Air-Pak 75", EquipmentType.BreathingApparatus_SCBA, true, DateTime.UtcNow);
        var workerId = Guid.NewGuid();
        const string permitNo = "PTW-OFFSHORE-9921";

        // Act
        var checkout = _equipmentService.CheckoutEquipment(item.Id, workerId, permitNo);

        // Assert
        Assert.NotNull(checkout);
        Assert.Equal(item.Id, checkout.EquipmentId);
        Assert.Equal(workerId, checkout.WorkerId);
        Assert.Equal(permitNo, checkout.PermitNumber);
        Assert.Equal(EquipmentStatus.CheckedOut, item.Status);
        Assert.Equal(permitNo, item.CurrentAssignedPermit);
        Assert.Equal(workerId, item.CurrentAssignedWorkerId);
    }

    [Fact]
    public void CheckoutEquipment_WhenCalibrationIsOverdue_ThrowsEquipmentOverdueException()
    {
        // Arrange
        var expiredDate = DateTime.UtcNow.AddDays(-60);
        var item = _equipmentService.RegisterEquipment("RAE-PID-009", "MiniRAE 3000 VOC", EquipmentType.GasDetector, true, expiredDate);
        var workerId = Guid.NewGuid();

        // Act & Assert
        var ex = Assert.Throws<EquipmentOverdueException>(() =>
        {
            _equipmentService.CheckoutEquipment(item.Id, workerId, "PTW-HAZMAT-101");
        });

        Assert.Contains("Safety Lockout", ex.Message);
        Assert.Equal(EquipmentStatus.Available, item.Status); // Status unchanged
    }

    [Fact]
    public void CheckinEquipment_WhenReturnedNormal_UpdatesHoursAndRestoresAvailableStatus()
    {
        // Arrange
        var item = _equipmentService.RegisterEquipment("CROW-T4-01", "Crowcon Tetra 4", EquipmentType.GasDetector, true, DateTime.UtcNow);
        var workerId = Guid.NewGuid();
        var checkout = _equipmentService.CheckoutEquipment(item.Id, workerId, "PTW-TANK-04");

        // Act
        var result = _equipmentService.CheckinEquipment(checkout.Id, operatingHoursAdded: 6.5, returnCondition: "Good condition, battery charged");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsReturned);
        Assert.Equal(6.5, result.HoursLogged);
        Assert.Equal(6.5, item.OperatingHours);
        Assert.Equal(EquipmentStatus.Available, item.Status);
        Assert.Null(item.CurrentAssignedPermit);
        Assert.Null(item.CurrentAssignedWorkerId);
    }

    [Fact]
    public void LotoIsolation_LockingAndZeroEnergyVerification_TracksStateAndPreventsUnauthorizedRemoval()
    {
        // Arrange
        var point = _equipmentService.RegisterIsolationPoint("ISO-VALVE-MOV-201", "High Pressure Crude Manifold Isolation", "ZONE_PROD_A", IsolationType.MechanicalValve);
        var permitId = Guid.NewGuid();
        var authorizedWorkerId = Guid.NewGuid();
        var unauthorizedWorkerId = Guid.NewGuid();
        const string lockSerial = "LOCK-RED-5541";

        // Act 1: Apply Padlock
        _equipmentService.ApplyLockout(point.Id, permitId, authorizedWorkerId, "John Rig Supervisor", lockSerial);

        // Assert 1
        Assert.Equal(IsolationState.LockedIsolated, point.State);
        Assert.Single(point.AppliedLocks);
        Assert.False(point.ZeroEnergyVerified);

        // Act 2: Verify Zero Energy
        _equipmentService.VerifyZeroEnergy(point.Id, "HSE-INSPECTOR-01");

        // Assert 2
        Assert.Equal(IsolationState.VerifiedZeroEnergy, point.State);
        Assert.True(point.ZeroEnergyVerified);

        // Act 3 & Assert 3: Unauthorized Worker cannot remove someone else's personal lock
        var ex = Assert.Throws<LotoViolationException>(() =>
        {
            _equipmentService.RemoveLockout(point.Id, unauthorizedWorkerId, lockSerial);
        });
        Assert.Contains("Unauthorized lock removal attempt", ex.Message);
        Assert.Single(point.AppliedLocks); // Lock remains in place

        // Act 4: Authorized Worker safely removes lock upon job completion
        _equipmentService.RemoveLockout(point.Id, authorizedWorkerId, lockSerial);
        Assert.Empty(point.AppliedLocks);
        Assert.Equal(IsolationState.OpenDeIsolated, point.State);
    }
}
