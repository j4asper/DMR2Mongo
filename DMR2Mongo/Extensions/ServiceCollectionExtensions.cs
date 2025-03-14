using DMR.NET.Options;
using DMR.NET.Services;
using DMR2Mongo.BackgroundServices;
using DMR2Mongo.Options;
using DMR2Mongo.Repositories;
using DMR2Mongo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DMR2Mongo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHostDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<DmrFtpOptions>()
            .BindConfiguration(DmrFtpOptions.DmrFtp)
            .ValidateDataAnnotations();
        
        services.AddOptionsWithValidateOnStart<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.Database)
            .ValidateDataAnnotations();
        
        services.AddOptionsWithValidateOnStart<DmrServiceOptions>()
            .BindConfiguration(DmrServiceOptions.DmrService)
            .ValidateDataAnnotations();

        services.AddSingleton<IMongoDatabase>(_ =>
            new MongoClient(configuration["Database:ConnectionString"])
                .GetDatabase(configuration["Database:DatabaseName"])
            );
        
        services.AddHostedService<DmrBackgroundService>();
        
        services
            .AddSingleton<IDmrRepository, DmrRepository>()
            .AddSingleton<IDmrService, DmrService>();
        
        return services;
    }
}