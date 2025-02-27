namespace Movies.Contracts.Requests;

public record RateMovieRequest
{
    public required int Rating { get; init; }
}