using System.ComponentModel.DataAnnotations;

namespace DMR2Mongo.Options;

public class DmrServiceOptions
{
    public static string DmrService = nameof(DmrService);
    
    [Required]
    public required int CheckIntervalHours { get; set; }
}