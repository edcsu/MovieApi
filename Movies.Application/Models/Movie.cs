using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }
    
    public required string Title { get; set; }

    public required int YearOfRelease { get; set; }

    public required List<string> Genres { get; init; } = [];

    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower().Replace(" ", "-");
        
        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^a-zA-Z0-9 _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
}