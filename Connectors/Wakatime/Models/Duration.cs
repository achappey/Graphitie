
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.WakaTime.Models;

public class Duration
{

    [JsonPropertyName("category")]
    public string Category { get; set; } = null!;

    [JsonPropertyName("time")]
    public float Start { get; set; }

    [JsonPropertyName("duration")]
    public float Seconds { get; set; }

    public float End
    {
        get
        {
            return Start + Seconds;
        }
    }


}