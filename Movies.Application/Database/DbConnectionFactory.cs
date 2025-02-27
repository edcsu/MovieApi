using System.Data;
using Npgsql;

namespace Movies.Application.Database;

public interface IDbConnectionFactory
{
    Task<IDbConnection> GetConnectionAsync();
}

public class NpgsqlDbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> GetConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}