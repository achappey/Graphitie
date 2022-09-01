using System.ComponentModel.DataAnnotations;

namespace Graphitie.Models;

public class License
{

    [Key]
    public string Id { get; set; } = string.Empty;

    public int? ConsumedUnits { get; set; }

    public int? EnabledUnits { get; set; }

    public Guid? SkuId { get; set; }

    public string? SkuPartNumber { get; set; }

    public string? AppliesTo { get; set; }
}