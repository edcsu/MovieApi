using Microsoft.AspNetCore.Mvc;
using Movie.Api.Mappings;
using Movies.Application.Repositories;
using Movies.Contracts.Requests;

namespace Movie.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMovieRequest request)
    {
        _logger.LogInformation("Creating movie ");
        var movie = request.MapToMovie();
        
        var result = await _movieRepository.CreateAsync(movie);
        _logger.LogInformation("Finished creating a movie");
        return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }
}