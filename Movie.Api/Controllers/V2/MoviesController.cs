using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movie.Api.Mappings;
using Movies.Application.Services;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movie.Api.Controllers.V2;

[ApiController]
[ApiVersion(2.0)]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly ILogger<MoviesController> _logger;
    private readonly IOutputCacheStore _outputCacheStore;

    public MoviesController(IMovieService movieService, ILogger<MoviesController> logger, IOutputCacheStore outputCacheStore)
    {
        _movieService = movieService;
        _logger = logger;
        _outputCacheStore = outputCacheStore;
    }

    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpPost(ApiEndpoints.V2.Movies.Create)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMovieRequest request,
        CancellationToken token = default)
    {
        _logger.LogInformation("Creating movie ");
        var movie = request.MapToMovie();
        
        var result = await _movieService.CreateAsync(movie, token);
        await _outputCacheStore.EvictByTagAsync("movies", token);
        _logger.LogInformation("Finished creating a movie");
        return result ? Created($"{ApiEndpoints.V1.Movies.Create}/{movie.Id}", movie) : BadRequest();
    }

    [AllowAnonymous]
    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> GetAsync([FromRoute] string idOrSlug,
        CancellationToken token = default)
    {
        var userId = HttpContext.GetUserId();
        
        _logger.LogInformation("Getting movie");
        var movie = Guid.TryParse(idOrSlug, out var movieId) ?
         await _movieService.GetByIdAsync(movieId, userId, token) :
         await _movieService.GetBySlugAsync(idOrSlug, userId, token);

        if (movie is null)
        {
            _logger.LogInformation("Movie not found");
            return NotFound();
        }
        _logger.LogInformation("Finished getting movie");
        return Ok(movie.MapToResponse());
    }
    
    [AllowAnonymous]
    [HttpGet(ApiEndpoints.V2.Movies.GetAll)]
    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
    [OutputCache(PolicyName = "MovieCache")]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetAllMoviesRequest request, 
        CancellationToken token = default)
    {
        _logger.LogInformation("Getting all movies");
        var userId = HttpContext.GetUserId();
        
        var options = request.MapToOptions()
            .WithUser(userId);
        var movies = await _movieService.GetAllAsync(options, token);
        var movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, token);

        var moviesResponse = movies.MapToMovieResponse(request.Page, request.PageSize, movieCount);

        _logger.LogInformation("Finished getting all movies");
        return Ok(moviesResponse);
    }

    [Authorize(ApiConstants.TrustedUserPolicy)]
    [HttpPut(ApiEndpoints.V2.Movies.Update)]
    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateMovieRequest request, [FromRoute] Guid id,
        CancellationToken token = default)
    {
        _logger.LogInformation("Updating movie");
        var userId = HttpContext.GetUserId();
        var movie = request.MapToMovie(id);
        
        var result = await _movieService.UpdateAsync(movie, userId, token);

        if (result is null)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
            return NotFound();
        }
        
        _logger.LogInformation("Finished updating movie");
        await _outputCacheStore.EvictByTagAsync("movies", token);

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(ApiConstants.AdminUserPolicy)]
    [HttpDelete(ApiEndpoints.V2.Movies.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken token = default)
    {
        _logger.LogInformation("Deleting movie");
        var result = await _movieService.DeleteByIdAsync(id, token);

        if (!result)
        {
            _logger.LogInformation("Movie not found with id:{Id}", id);
        }
        
        await _outputCacheStore.EvictByTagAsync("movies", token);
        _logger.LogInformation("Finished deleting movie");
        return Ok();
    }
}