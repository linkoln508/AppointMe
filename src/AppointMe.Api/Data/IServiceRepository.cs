using AppointMe.Api.Models;

namespace AppointMe.Api.Data
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllByBusinessAsync(Guid businessId);

        Task<Service?> GetByIdAsync(Guid businessId, Guid serviceId);

        Task<Guid> CreateAsync(Service service);

        Task<bool> UpdateAsync(Service service);

        Task<bool> DeleteAsync(Guid businessId, Guid serviceId);
    }
}