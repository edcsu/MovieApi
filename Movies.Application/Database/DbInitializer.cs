using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _connectionFactory.GetConnectionAsync();

        await connection.ExecuteAsync("""
              CREATE TABLE IF NOT EXISTS movies(
              id UUID primary key,
              slug TEXT not null,
              title TEXT not null,
              yearofrelease integer not null);
        """);

        await connection.ExecuteAsync("""
              CREATE unique index concurrently if not exists movies_slug_idx
              on movies
              using btree(slug);
        """);
    }
}