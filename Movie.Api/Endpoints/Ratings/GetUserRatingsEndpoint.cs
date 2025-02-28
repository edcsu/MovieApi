using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movie.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";
    
    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.V1.Ratings.GetUserRatings,
                async (HttpContext context, IRatingService ratingService,
                    CancellationToken token) =>
                {
                    var userId = context.GetUserId();
                    var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, token);
                    return TypedResults.Ok(ratings);
                })
            .WithName(Name)
            .Produces<MovieRatingResponse>(StatusCodes.Status200OK)
            .RequireAuthorization();
        
        return app;
    }
}