using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AppointMe.Api.Data;

public class EfWorkingHoursRepository : IWorkingHoursRepository
{
    private readonly AppointMeDbContext _db;
    public EfWorkingHoursRepository(AppointMeDbContext db)
    {
        _db = db;
    }
    public async Task<IEnumerable<WorkingHours>> GetByBusinessIdAsync(Guid businessId)
    {
        return await _db.WorkingHours
            .AsNoTracking()
            .Where(day => day.BusinessId == businessId)
            .OrderBy(day => day.DayOfWeek)
            .ToListAsync();
    }
}