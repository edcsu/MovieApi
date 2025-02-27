using System.Data;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Movies.Application.Database;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Npgsql;

namespace Movies.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IMovieRepository, MovieRepository>();
        services.AddSingleton<IRatingRepository, RatingRepository>();
        services.AddSingleton<IMovieService, MovieService>();
        services.AddValidatorsFromAssemblyContaining<IAppicaltionMarker>(ServiceLifetime.Singleton);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => 
            new NpgsqlDbConnectionFactory(connectionString));
        services.AddSingleton<DbInitializer>();
        return services;
    }
}