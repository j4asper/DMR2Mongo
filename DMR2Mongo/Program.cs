using DMR.NET.Extensions;
using DMR2Mongo.Extensions;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostContext, serviceCollection) =>
{
    serviceCollection.AddHostDependencies(hostContext.Configuration);
});

builder.UseDmrDotNet();

builder.UseSerilog();

var app = builder.Build();

await app.RunAsync();