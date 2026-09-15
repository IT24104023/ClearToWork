using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Workforce;

public class CertificateType : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string RequiredForTrade { get; set; } = string.Empty;
    public int ValidityMonths { get; set; } = 12;
}