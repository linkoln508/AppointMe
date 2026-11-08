using AppointMe.Api.Models;

namespace AppointMe.Api.Data
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();

        Task<Category?> GetByIdAsync(int id);

        // Новый id
        Task<int> CreateAsync(Category category);

        Task<bool> UpdateAsync(Category category);

        Task<bool> DeleteAsync(int id);
    }
}
