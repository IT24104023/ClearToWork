using ClearToWork.Domain.Common;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Domain.Entities.Identity;

public class User : BaseAuditableEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public Guid? ContractorId { get; set; }
    public bool IsActive { get; set; } = true;
}
