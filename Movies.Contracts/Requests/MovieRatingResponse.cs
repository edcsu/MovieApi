namespace Movies.Contracts.Requests;

public record MovieRatingResponse
{
    public required Guid MovieId { get; init; }
    
    public required string Slug { get; init; }
    
    public required int Rating { get; init; }
}