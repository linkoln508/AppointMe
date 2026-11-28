using AppointMe.Api.Models;

namespace AppointMe.Api.Data;
public interface IWorkingHoursRepository
{
    Task<IEnumerable<WorkingHours>> GetByBusinessIdAsync(Guid businessId);

}