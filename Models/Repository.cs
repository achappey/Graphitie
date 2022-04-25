

namespace Graphitie.Models;

public class Repository
{
    public string FullName { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public string Description { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int Size { get; set; }

    public string Language { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

}