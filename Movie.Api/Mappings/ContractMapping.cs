using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movie.Api.Mappings;

public static class ContractMapping
{
    public static Movies.Application.Models.Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movies.Application.Models.Movie
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList(),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public static MovieResponse MapToResponse(this Movies.Application.Models.Movie movie)
    {
        return new MovieResponse
        {
            Id = Guid.CreateVersion7(),
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.ToList()
        };
    }
    
    public static MoviesResponses MapToMovieResponse(this IEnumerable<Movies.Application.Models.Movie> movies)
    {
        return new MoviesResponses
        {
            Items = movies.Select(MapToResponse)
        };
    }
}