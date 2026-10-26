namespace TaskApi;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool IsDone { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

public record TodoInput(string? Title, bool IsDone);
