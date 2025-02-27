using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movie.Api.Mappings;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movie.Api.Controllers;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger)
    {
        _movieService = movieService;
        _logger = logger;
    }

    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpPost(ApiEndpoints.Movies.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMovieRequest request,
        CancellationToken token = default)
    {
        _logger.LogInformation("Creating movie ");
        var movie = request.MapToMovie();
        
        var result = await _movieService.CreateAsync(movie, token);
        _logger.LogInformation("Finished creating a movie");
        return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> GetAsync([FromRoute] string idOrSlug,
        CancellationToken token = default)
    {
        _logger.LogInformation("Getting movie");
        var movie = Guid.TryParse(idOrSlug, out var movieId) ?
         await _movieService.GetByIdAsync(movieId, token) :
         await _movieService.GetBySlugAsync(idOrSlug, token);

        if (movie is null)
        {
            _logger.LogInformation("Movie not found");
            return NotFound();
        }
        _logger.LogInformation("Finished getting movie");
        return Ok(movie.MapToResponse());
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAllAsync(CancellationToken token = default)
    {
        _logger.LogInformation("Getting all movies");
        var movies = await _movieService.GetAllAsync(token);

        _logger.LogInformation("Finished getting all movies");
        return Ok(movies.MapToMovieResponse());
    }

    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateMovieRequest request, [FromRoute] Guid id,
        CancellationToken token = default)
    {
        _logger.LogInformation("Updating movie");
        var movie = request.MapToMovie(id);
        var result = await _movieService.UpdateAsync(movie, token);

        if (result is null)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
            return NotFound();
        }
        
        _logger.LogInformation("Finished updating movie");
        
        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(ApiConstants.AdminUserPolicy)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token = default)
    {
        _logger.LogInformation("Deleting movie");
        var result = await _movieService.DeleteByIdAsync(id, token);

        if (!result)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
        }
        
        _logger.LogInformation("Finished deleting movie");
        return Ok();
    }
}