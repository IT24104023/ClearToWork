using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Workforce;

public class Contractor : BaseAuditableEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public bool IsSuspended { get; set; } = false;

    public ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
