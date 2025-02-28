using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movie.Api.Health;

public class DatabaseHealthCheck : IHealthCheck
{
    public const string Name = "Database";
    
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken token = new())
    {
        try
        {
            _ = await _dbConnectionFactory.GetConnectionAsync(token);
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            const string errorMessage = "Database is unhealthy";
            _logger.LogError(errorMessage, e);
            return HealthCheckResult.Unhealthy(errorMessage, e);
        }
    }
}
