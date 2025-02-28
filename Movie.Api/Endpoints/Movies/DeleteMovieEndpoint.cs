using Microsoft.AspNetCore.OutputCaching;
using Movies.Application.Services;

namespace Movie.Api.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.V1.Movies.Delete, async (
                Guid id, IMovieService movieService,
                IOutputCacheStore outputCacheStore, CancellationToken token) =>
            {
                var deleted = await movieService.DeleteByIdAsync(id, token);
                if (!deleted)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync("movies", token);
                return Results.Ok();
            })
            .WithName(Name)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(ApiConstants.AdminUserPolicy);
        return app;
    }
}