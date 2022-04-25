namespace Graphitie.Models;

public class UserRegistrationDetails 
{
    public string Id { get; set; } = string.Empty;

    public string? UserPrincipalName { get; set; }
    public bool IsSsprRegistered { get; set; }
    public bool IsSsprEnabled { get; set; }
    public bool IsSsprCapable { get; set; }
    public bool IsMfaRegistered { get; set; }
    public bool IsMfaCapable { get; set; }
    public bool IsPasswordlessCapable { get; set; }
    public IEnumerable<string>? MethodsRegistered { get; set; }

}
