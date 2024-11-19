namespace Lab9.Models;

public record History : Model
{
    public required string Title { get; init; }

    public required string Description { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}
