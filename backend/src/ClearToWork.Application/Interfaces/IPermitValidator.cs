using System.Threading.Tasks;
using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Entities.Permits;
using ClearToWork.Domain.Common;
using ClearToWork.Domain.Entities.Equipment;
using ClearToWork.Domain.Entities.Hazards;
using ClearToWork.Domain.Entities.Identity;
using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Application.Interfaces;

public interface IPermitValidator
{
    Task<ValidationReportDto> ValidatePermitRulesAsync(PermitRequest permit);
}
