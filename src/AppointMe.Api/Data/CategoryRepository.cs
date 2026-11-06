using AppointMe.Api.Models;
using Dapper;
using Npgsql;

namespace AppointMe.Api.Data
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public CategoryRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "SELECT id, name, slug FROM categories ORDER BY name";

            return await connection.QueryAsync<Category>(sql);
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "SELECT id, name, slug FROM categories WHERE id = @Id";

            return await connection.QuerySingleOrDefaultAsync<Category>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Category category)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "INSERT INTO categories(name, slug) VALUES(@Name, @Slug) RETURNING id";

            return await connection.ExecuteScalarAsync<int>(sql, category);
        }
        public async Task<bool> UpdateAsync(Category category)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "UPDATE categories SET name = @Name, slug = @Slug WHERE id = @Id";

            var rows = await connection.ExecuteAsync(sql, category);

            return rows > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            await using var connection = await _dataSource.OpenConnectionAsync();

            const string sql = "DELETE FROM categories WHERE id = @Id";

            var rows = await connection.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
    }
}