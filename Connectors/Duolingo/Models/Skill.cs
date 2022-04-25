
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.Duolingo.Models;

public class Skill
{
    [JsonPropertyName("language_string")]
    public string LanguageName { get; set; } = null!;

    [JsonPropertyName("language")]
    public string LanguageCode { get; set; } = null!;

    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; } = null!;

}