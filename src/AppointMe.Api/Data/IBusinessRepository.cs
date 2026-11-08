using AppointMe.Api.Models;

namespace AppointMe.Api.Data
{
    public interface IBusinessRepository
    {
        Task<IEnumerable<Business>> GetAllAsync();

        // null, если не нашли.
        Task<Business?> GetByIdAsync(Guid id);

        Task<Guid> CreateAsync(Business business);

        Task<bool> UpdateAsync(Business business);

        Task<bool> DeleteAsync(Guid id);
    }
}