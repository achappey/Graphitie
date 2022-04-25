
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.Duolingo.Models;

public class LanguageData
{
    [JsonPropertyName("skills")]
    public IEnumerable<Skill> Skills { get; set; } = null!;

    [JsonPropertyName("language_string")]
    public string Language { get; set; } = null!;

    [JsonPropertyName("language")]
    public string Code { get; set; } = null!;

    [JsonPropertyName("calendar")]
    public IEnumerable<Calendar> Calendar { get; set; } = null!;


}