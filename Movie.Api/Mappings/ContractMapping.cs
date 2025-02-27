using Movies.Application.Models;
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
            Genres = request.Genres.ToList()
        };
    }
    
    public static Movies.Application.Models.Movie MapToMovie(this UpdateMovieRequest request, Guid movieId)
    {
        return new Movies.Application.Models.Movie
        {
            Id = movieId,
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
    }
    
    public static MovieResponse MapToResponse(this Movies.Application.Models.Movie movie)
    {
        return new MovieResponse
        {
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Genres = movie.Genres.ToList(),
            Slug = movie.Slug,
            Rating = movie.Rating,
            UserRating = movie.UserRating,
        };
    }
    
    public static MoviesResponses MapToMovieResponse(this IEnumerable<Movies.Application.Models.Movie> movies)
    {
        return new MoviesResponses
        {
            Items = movies.Select(MapToResponse)
        };
    }
    
    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(x => new MovieRatingResponse
        {
            Rating = x.Rating,
            Slug = x.Slug,
            MovieId = x.MovieId
        });
    }
}