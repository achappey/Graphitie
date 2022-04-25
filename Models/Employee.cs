namespace Graphitie.Models;

public class Employee
{
    public IEnumerable<Language> Languages { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Mail { get; set; }

}