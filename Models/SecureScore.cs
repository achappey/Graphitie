namespace Graphitie.Models;

public class SecureScore
{
    public string Id { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public double ComparativeScore { get; set; }

}
