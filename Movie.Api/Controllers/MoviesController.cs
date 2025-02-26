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

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> GetAsync([FromRoute] string idOrSlug)
    {
        _logger.LogInformation("Getting movie");
        var movie = Guid.TryParse(idOrSlug, out var movieId) ?
         await _movieRepository.GetByIdAsync(movieId) :
         await _movieRepository.GetBySlugAsync(idOrSlug);

        if (movie is null)
        {
            _logger.LogInformation("Movie not found");
            return NotFound();
        }
        _logger.LogInformation("Finished getting movie");
        return Ok(movie.MapToResponse());
    }
    
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAllAsync()
    {
        _logger.LogInformation("Getting all movies");
        var movies = await _movieRepository.GetAllAsync();

        _logger.LogInformation("Finished getting all movies");
        return Ok(movies.MapToMovieResponse());
    }

    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateMovieRequest request, [FromRoute] Guid id)
    {
        _logger.LogInformation("Updating movie");
        var movie = request.MapToMovie(id);
        var result = await _movieRepository.UpdateAsync(movie);

        if (!result)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
            return NotFound();
        }
        
        _logger.LogInformation("Finished updating movie");
        
        var response = movie.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        _logger.LogInformation("Deleting movie");
        var result = await _movieRepository.DeleteByIdAsync(id);

        if (!result)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
        }
        
        _logger.LogInformation("Finished deleting movie");
        return Ok();
    }
}