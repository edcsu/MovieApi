using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

    public MovieService(IMovieRepository movieRepository, 
        IValidator<Movie> movieValidator, 
        IRatingRepository ratingRepository, IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
        _ratingRepository = ratingRepository;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateAsync(movie, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }
 
    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,
        CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, token);
        
        return await _movieRepository.GetAllAsync(options, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);

        if (!movieExists)
        {
            return null;
        }
        
        await _movieRepository.UpdateAsync(movie, cancellationToken); 
        
        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
            movie.Rating = rating;
            return movie;
        }
        
        var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cancellationToken);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }
    
    public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default)
    {
        return _movieRepository.GetCountAsync(title, yearOfRelease, token);
    }
}