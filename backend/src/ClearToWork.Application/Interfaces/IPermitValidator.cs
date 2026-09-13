using ClearToWork.Application.DTOs;
using ClearToWork.Domain.Entities.Permits;

namespace ClearToWork.Application.Interfaces;

public interface IPermitValidator
{
    Task<ValidationReportDto> ValidatePermitRulesAsync(PermitRequest permit);
}
