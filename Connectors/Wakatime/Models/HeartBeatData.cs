
using System.Text.Json.Serialization;

namespace Graphitie.Connectors.WakaTime.Models;

public class HeartBeatData
{
    [JsonPropertyName("end")]
    public DateTimeOffset End { get; set; }

    [JsonPropertyName("data")]
    public IEnumerable<HeartBeat> Data { get; set; } = null!;

   
}
