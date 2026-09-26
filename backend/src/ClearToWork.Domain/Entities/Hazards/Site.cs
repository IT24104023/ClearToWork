using System;
using System.Collections.Generic;
using ClearToWork.Domain.Common;

namespace ClearToWork.Domain.Entities.Hazards;

public class Site : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
