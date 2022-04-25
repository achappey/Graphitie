
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.WakaTime.Models;

public class HeartBeat
{

    [JsonPropertyName("language")]
    public string Language { get; set; } = null!;

    [JsonPropertyName("category")]
    public string Category { get; set; } = null!;

    [JsonPropertyName("time")]
    public float Time { get; set; }

    [JsonPropertyName("lines")]
    public int Lines { get; set; }


}