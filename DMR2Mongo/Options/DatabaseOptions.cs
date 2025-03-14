using System.ComponentModel.DataAnnotations;

namespace DMR2Mongo.Options;

public class DatabaseOptions
{
    public static string Database = nameof(Database);
    
    [Required]
    public required string ConnectionString { get; set; }
    
    [Required]
    public required string DatabaseName { get; set; }
}