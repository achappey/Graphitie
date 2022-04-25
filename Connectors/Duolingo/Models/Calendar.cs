
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.Duolingo.Models;

public class Calendar
{
    [JsonPropertyName("skill_id")]
    public string Skill { get; set; } = null!;

    [JsonPropertyName("event_type")]
    public string EventType { get; set; } = null!;

    [JsonPropertyName("improvement")]
    public int Improvement { get; set; }

    [JsonPropertyName("datetime")]
    public long DateTime { get; set; }


}