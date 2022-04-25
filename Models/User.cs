namespace Graphitie.Models;

public class User
{

    public IEnumerable<Language> Languages { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? MobilePhone { get; set; }    
    public string? Mail { get; set; }
    public string? UserPrincipalName { get; set; }
    public string? EmployeeId { get; set; }
    public string? CompanyName { get; set; }
    public DateTimeOffset? CreatedDateTime { get; set; }
    public bool AccountEnabled { get; set; }

}