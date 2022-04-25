namespace Graphitie.Models;

public class SecurityAlert 
{
    public string Id { get; set; } = string.Empty;

    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Severity { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }

    public IEnumerable<string> Users { get; set; } = new List<string>();



}
