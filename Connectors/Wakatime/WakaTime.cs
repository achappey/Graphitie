
using Graphitie.Connectors.WakaTime.Models;

namespace Graphitie.Connectors.WakaTime;

public interface IWakaTime
{
    public Task<IEnumerable<HeartBeat>> GetHeartBeats(string apiKey, DateTime date);
    public Task<IEnumerable<Duration>> GetDurations(string apiKey, DateTime date);

}

public class WakaTime : IWakaTime
{
    private readonly HttpClient _httpClient = null!;
    private const string BaseAddress = "https://wakatime.com";

    public WakaTime(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri(BaseAddress);
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<HeartBeat>> GetHeartBeats(string apiKey, DateTime date)
    {
        var json = await GetData<HeartBeatData>(apiKey, string.Format("/api/v1/users/current/heartbeats?date={0}", date.ToString("o")));

        return json != null && json.Data != null ? json.Data : new List<HeartBeat>();
    }

    public async Task<IEnumerable<Duration>> GetDurations(string apiKey, DateTime date)
    {
        var json = await GetData<DurationData>(apiKey, string.Format("/api/v1/users/current/durations?date={0}&slice_by=category", date.ToString("o")));

        return json != null && json.Data != null ? json.Data : new List<Duration>();
    }

    private async Task<T?> GetData<T>(string apiKey, string url)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(apiKey);

        _httpClient.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", System.Convert.ToBase64String(plainTextBytes)));

        var getUserDataResult = await _httpClient.GetAsync(url);

        getUserDataResult.EnsureSuccessStatusCode();

        return await getUserDataResult.Content.ReadFromJsonAsync<T>();
    }


}