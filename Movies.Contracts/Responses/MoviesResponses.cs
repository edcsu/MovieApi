namespace Movies.Contracts.Responses;

public record MoviesResponses
{
    public required IEnumerable<MovieResponse> Items { get; init; } = [];
}