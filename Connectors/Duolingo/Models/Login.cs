using System.Text.Json.Serialization;

namespace Graphitie.Connectors.Duolingo.Models;

public class Login {

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
    
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = null!;



}