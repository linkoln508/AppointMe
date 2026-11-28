using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using AppointMe.Api.Models.Admin;

namespace AppointMe.Api.Data;

public class EfBusinessRepository : IBusinessRepository
{
    private readonly AppointMeDbContext _db;

    public EfBusinessRepository(AppointMeDbContext db)
    {
        _db = db;
    }

    private IQueryable<Business> BusinessesWithCategoryName()
    {
        return _db.Businesses
            .AsNoTracking()
            .Join(_db.Categories,
                b => b.CategoryId,
                c => c.Id,
                (b, c) => new Business
                {
                    Id = b.Id,
                    OwnerId = b.OwnerId,
                    CategoryId = b.CategoryId,
                    CategoryName = c.Name,
                    Name = b.Name,
                    Description = b.Description,
                    City = b.City,
                    Address = b.Address,
                    Phone = b.Phone,
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt
                });
    }

    public async Task<IEnumerable<Business>> GetAllAsync()
    {
        return await BusinessesWithCategoryName()
            .Where(b => b.IsActive)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<Business?> GetByIdAsync(Guid id)
    {
        return await BusinessesWithCategoryName()
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    public async Task<IEnumerable<Business>> GetByOwnerAsync(Guid ownerId)
    {
        return await BusinessesWithCategoryName()
            .Where(b => b.OwnerId == ownerId)
            .OrderBy(b => b.Name)
            .ToListAsync();
    }

    // репозиторий бизнесов
    public async Task<Business?> GetByIdWithServicesAsync(Guid id)
    {
        return await _db.Businesses
            .AsNoTracking()
            .Join(_db.Categories,
                b => b.CategoryId,
                c => c.Id,
                (b, c) => new Business
                {
                    Id = b.Id,
                    OwnerId = b.OwnerId,
                    CategoryId = b.CategoryId,
                    CategoryName = c.Name,
                    Name = b.Name,
                    Description = b.Description,
                    City = b.City,
                    Address = b.Address,
                    Phone = b.Phone,
                    IsActive = b.IsActive,
                    CreatedAt = b.CreatedAt,
                    Services = b.Services.Where(s => s.IsActive).OrderBy(s => s.Name).ToList()
                })
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Guid> CreateAsync(Business business)
    {
        _db.Businesses.Add(business);
        await SaveAsync();
        return business.Id;
    }

    public async Task<bool> UpdateAsync(Business business)
    {
        var existing = await _db.Businesses.FindAsync(business.Id);
        if (existing == null)
        {
            return false;
        }

        existing.CategoryId = business.CategoryId;
        existing.Name = business.Name;
        existing.Description = business.Description;
        existing.City = business.City;
        existing.Address = business.Address;
        existing.Phone = business.Phone;
        existing.IsActive = business.IsActive;
        await SaveAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _db.Businesses.FindAsync(id);
        if (existing == null)
        {
            return false;
        }

        _db.Businesses.Remove(existing);
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

        public async Task<List<AdminBusinessResponse>> GetAllForAdminAsync()
    {
        var query =
            from b in _db.Businesses.AsNoTracking()
            join c in _db.Categories on b.CategoryId equals c.Id
            join u in _db.Users on b.OwnerId equals (Guid?)u.Id into owners
            from owner in owners.DefaultIfEmpty()
            orderby b.Name
            select new AdminBusinessResponse
            {
                Id = b.Id,
                Name = b.Name,
                CategoryName = c.Name,
                City = b.City,
                IsActive = b.IsActive,
                OwnerEmail = owner != null ? owner.Email : null
            };

        return await query.ToListAsync();
    }

    public async Task<bool> SetActiveAsync(Guid id, bool isActive)
    {
        var business = await _db.Businesses.FindAsync(id);
        if (business == null)
        {
            return false;
        }

        business.IsActive = isActive;
        await _db.SaveChangesAsync();
        return true;
    }

    Task<IEnumerable<Business>> IBusinessRepository.GetAllAsync()
    {
        throw new NotImplementedException();
    }
}