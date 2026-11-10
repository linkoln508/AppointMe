using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AppointMe.Api.Data;

public class EfServiceRepository : IServiceRepository
{
    private readonly AppointMeDbContext _db;

    public EfServiceRepository(AppointMeDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Service>> GetAllByBusinessAsync(Guid businessId)
    {
        return await _db.Services
            .AsNoTracking()
            .Where(s => s.BusinessId == businessId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<Service?> GetByIdAsync(Guid businessId, Guid serviceId)
    {
        return await _db.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.BusinessId == businessId);
    }

    public async Task<Guid> CreateAsync(Service service)
    {
        _db.Services.Add(service);
        await SaveAsync();
        return service.Id;
    }

    public async Task<bool> UpdateAsync(Service service)
    {
        var existing = await _db.Services
            .FirstOrDefaultAsync(s => s.Id == service.Id && s.BusinessId == service.BusinessId);
        if (existing == null)
        {
            return false;
        }

        existing.Name = service.Name;
        existing.Description = service.Description;
        existing.Price = service.Price;
        existing.DurationMinutes = service.DurationMinutes;
        existing.IsActive = service.IsActive;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid businessId, Guid serviceId)
    {
        var existing = await _db.Services
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.BusinessId == businessId);
        if (existing == null)
        {
            return false;
        }

        _db.Services.Remove(existing);
        await SaveAsync();
        return true;
    }

    private async Task SaveAsync()
    {
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            throw pg;
        }
    }
}