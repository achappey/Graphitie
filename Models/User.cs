namespace Graphitie.Models;

public class User
{

    public string Id { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? GivenName { get; set; }
    public string? Surname { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? MobilePhone { get; set; }    
    public string? Mail { get; set; }
    public string? UserPrincipalName { get; set; }
    public string? EmployeeId { get; set; }
    public string? CompanyName { get; set; }
    public string? UserType { get; set; }
    public bool AccountEnabled { get; set; }
    public DateTimeOffset? CreatedDateTime { get; set; }
    public IEnumerable<string> AssignedLicenses { get; set; } = null!;

}