namespace Graphitie.Models;

public class SignIn 
{
    public string Id { get; set; } = string.Empty;
    public string? AppDisplayName { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public string? UserPrincipalName { get; set; }
    public SignInLocation? Location { get; set; }
    public SignInStatus? Status { get; set; }

}

public class SignInLocation 
{
    public string? City { get; set; }
    public string? CountryOrRegion { get; set; }
    public string? State { get; set; }
}


public class SignInStatus 
{
    public int ErrorCode { get; set; }
    public string? FailureReason { get; set; }    
}
