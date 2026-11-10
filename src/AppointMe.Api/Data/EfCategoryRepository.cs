using AppointMe.Api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AppointMe.Api.Data;
public class EfCategoryRepository : ICategoryRepository
{
    private readonly AppointMeDbContext _db;

    // DbContext даст DI
    public EfCategoryRepository(AppointMeDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await SaveAsync();
        return category.Id;
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        var existing = await _db.Categories.FindAsync(category.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Name = category.Name;
        existing.Slug = category.Slug;
        await SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Categories.FindAsync(id);
        if (existing == null)
        {
            return false;
        }

        _db.Categories.Remove(existing);
        await SaveAsync();
        return true;
    }

    // сохранить и распаковать ошибку PostgreSQL
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