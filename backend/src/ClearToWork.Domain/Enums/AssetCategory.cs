using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Enums;

public enum AssetCategory
{
    GasMonitor,
    GasDetector,
    SCBA,
    IsolationDevice,
    HeavyMachinery,
    ElectricalTool,
    FireExtinguisher,
    General
}