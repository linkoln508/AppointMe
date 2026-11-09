using AppointMe.Api.Models;
using Dapper;
using Npgsql;
using System.Dynamic;

namespace AppointMe.Api.Data
{
    public class BusinessRepository : IBusinessRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public BusinessRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<IEnumerable<Business>> GetAllAsync()
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @" SELECT b.id, b.category_id, c.name AS category_name, b.name, b.description,
                                b.city, b.address, b.phone, b.is_active, b.created_at FROM businesses b
                                JOIN categories c ON b.category_id = c.id WHERE b.is_active = true ORDER BY b.name;";

            return await connection.QueryAsync<Business>(sql);
        }

        public async Task<Business?> GetByIdAsync(Guid id)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @"SELECT b.id, b.category_id, c.name AS category_name, b.name, b.description,
                                b.city, b.address, b.phone, b.is_active, b.created_at
                                FROM businesses b JOIN categories c ON b.category_id = c.id WHERE b.id = @Id;";

            return await connection.QuerySingleOrDefaultAsync<Business>(sql, new { Id = id });
        }

        public async Task<Guid> CreateAsync(Business business)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            // id и created_at ставит база
            const string sql = @"INSERT INTO businesses (category_id, name, description, city, address, phone, is_active)
                                VALUES (@CategoryId, @Name, @Description, @City, @Address, @Phone, @IsActive)
                                RETURNING id";

            return await connection.ExecuteScalarAsync<Guid>(sql, business);
        }

        public async Task<bool> UpdateAsync(Business business)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @" UPDATE businesses SET category_id = @CategoryId, name = @Name, description = @Description,
                                city = @City, address = @Address, phone = @Phone, is_active = @IsActive
                                WHERE id = @Id";

            var rows = await connection.ExecuteAsync(sql, business);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            // (ON DELETE CASCADE).
            const string sql = "DELETE FROM businesses WHERE id = @Id";

            var rows = await connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }

        public async Task<Business?> GetByIdWithServicesAsync(Guid id)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = @"SELECT b.id, b.category_id, c.name AS category_name, b.name, b.description,
                                b.city, b.address, b.phone, b.is_active, b.created_at
                                FROM businesses b JOIN categories c ON b.category_id = c.id WHERE b.id = @Id;
                                SELECT id, business_id, name, description, price, duration_minutes, is_active
                                FROM services WHERE business_id = @Id AND is_active = true ORDER BY name";
            using var multi = await connection.QueryMultipleAsync(sql, new { Id = id });

            var business = await multi.ReadSingleOrDefaultAsync<Business>();
            var services = await multi.ReadAsync<Service>();

            if (business == null)
            {
                return null;
            }

            business.Services = services.ToList();

            return business;
        }
    }
}