using Movies.Contracts.Requests;

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
            Genres = request.Genres.ToList()
        };
    }
}