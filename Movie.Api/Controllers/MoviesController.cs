using Microsoft.AspNetCore.Mvc;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movie.Api.Controllers;

[ApiController]
[Route("api")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    [HttpPost("movies")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMovieRequest request)
    {
        _logger.LogInformation("Creating movie ");
        var movie = new Movies.Application.Models.Movie
        {
            Id = Guid.CreateVersion7(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease,
            Genres = request.Genres.ToList()
        };
        
        var result = await _movieRepository.CreateAsync(movie);
        _logger.LogInformation("Finished creating a movie");
        return Created($"/api/movies/{movie.Id}", movie);
    }
}