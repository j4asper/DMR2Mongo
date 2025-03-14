using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DMR2Mongo.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseDefaultServiceProvider(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseDefaultServiceProvider(options =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });
        
        return hostBuilder;
    }
    
    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((hostContext, logging) =>
        {
            logging.ClearProviders();
            
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(hostContext.Configuration)
                .CreateLogger();
            
            logging.AddSerilog();
        });
        
        return hostBuilder;
    }
}