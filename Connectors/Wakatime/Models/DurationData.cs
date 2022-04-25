
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.WakaTime.Models;

public class DurationData
{
    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    [JsonPropertyName("data")]
    public IEnumerable<Duration> Data { get; set; } = null!;

   
}
