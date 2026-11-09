using AppointMe.Api.Models;
using Dapper;
using Npgsql;

namespace AppointMe.Api.Data
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public ServiceRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Service>> GetAllByBusinessAsync(Guid businessId)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                SELECT id, business_id, name, description, price, duration_minutes, is_active
                FROM services
                WHERE business_id = @BusinessId
                ORDER BY name";

            return await connection.QueryAsync<Service>(sql, new { BusinessId = businessId });
        }

        public async Task<Service?> GetByIdAsync(Guid businessId, Guid serviceId)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                SELECT id, business_id, name, description, price, duration_minutes, is_active
                FROM services
                WHERE id = @ServiceId AND business_id = @BusinessId";

            return await connection.QuerySingleOrDefaultAsync<Service>(
                sql, new { ServiceId = serviceId, BusinessId = businessId });
        }

        public async Task<Guid> CreateAsync(Service service)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @"
                INSERT INTO services (business_id, name, description, price, duration_minutes, is_active)
                VALUES (@BusinessId, @Name, @Description, @Price, @DurationMinutes, @Is_active)
                RETURNING id";

            return await connection.ExecuteScalarAsync<Guid>(sql, service);
        }

        public async Task<bool> UpdateAsync(Service service)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            // в WHERE: нельзя изменить чужую услугу.
            const string sql = @"
                UPDATE services
                SET name = @Name, description = @Description, price = @Price, duration_minutes = @DurationMinutes, is_active = @IsActive
                WHERE id = @Id AND business_id = @BusinessId";

            var rows = await connection.ExecuteAsync(sql, service);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid businessId, Guid serviceId)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "DELETE FROM services WHERE id = @ServiceId AND business_id = @BusinessId";

            var rows = await connection.ExecuteAsync(sql, new { ServiceId = serviceId, BusinessId = businessId });

            return rows > 0;
        }
    }
}