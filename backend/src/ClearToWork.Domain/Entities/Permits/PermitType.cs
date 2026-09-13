using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Permits;

public class PermitType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxDurationHours { get; set; } = 8;
    public bool RequiresFireWatch { get; set; } = false;
}
