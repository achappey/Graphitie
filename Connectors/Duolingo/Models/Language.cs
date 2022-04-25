
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.Duolingo.Models;

public class Language
{


    [JsonPropertyName("language_string")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("language")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("points")]
    public int Points { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

}