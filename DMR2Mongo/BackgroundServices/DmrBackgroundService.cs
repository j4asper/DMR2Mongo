using DMR.NET.Entities.Models;
using DMR.NET.Options;
using DMR.NET.Services;
using DMR2Mongo.Options;
using DMR2Mongo.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DMR2Mongo.BackgroundServices;

public class DmrBackgroundService : BackgroundService
{
    private readonly ILogger<DmrBackgroundService> _logger;
    private readonly IDmrFtpService _dmrFtpService;
    private readonly IDmrService _dmrService;
    private readonly IDmrDeserializerService _dmrDeserializerService;
    private readonly TimeSpan _checkInterval;
    private readonly DmrFtpOptions _options;
    
    public DmrBackgroundService(
        IDmrFtpService dmrFtpService,
        IDmrDeserializerService dmrDeserializerService,
        ILogger<DmrBackgroundService> logger,
        IDmrService dmrService,
        IOptions<DmrServiceOptions> dmrServiceOptions,
        IOptions<DmrFtpOptions> dmrFtpOptions)
    {
        _dmrFtpService = dmrFtpService;
        _dmrDeserializerService = dmrDeserializerService;
        _logger = logger;
        _dmrService = dmrService;
        _options = dmrFtpOptions.Value;
        _checkInterval = TimeSpan.FromHours(dmrServiceOptions.Value.CheckIntervalHours);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var latestDmrDatabaseAvailable = await _dmrFtpService.LatestDmrDatabaseAvailable(stoppingToken);

                if (!latestDmrDatabaseAvailable)
                {
                    _logger.LogInformation("Found new DMR database available");

                    await RemoveOldDatabaseAsync(stoppingToken);
                    
                    await _dmrFtpService.DownloadDmrDatabaseAsync(stoppingToken);
                    
                    try
                    {
                        var enumerableTask = _dmrDeserializerService.DeserializeDmrEntriesAsync(stoppingToken);

                        HashSet<string> idsInserted = [];
                        List<DmrEntry> entries = [];

                        var i = 0;

                        await foreach (var item in enumerableTask)
                        {
                            if (idsInserted.Contains(item.Id))
                                continue;
                            
                            entries.Add(item);
                            
                            idsInserted.Add(item.Id);
                            
                            i++;

                            if (i > 100000)
                            {
                                i = 0;
                                
                                await _dmrService.InsertOrUpdateManyAsync(entries, stoppingToken);
                                
                                entries.Clear();
                            }
                        }
                        
                        await _dmrService.InsertOrUpdateManyAsync(entries, stoppingToken);
                        
                        _logger.LogInformation("Finished writing entries to database.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred in {service}", nameof(DmrBackgroundService));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in {service}", nameof(DmrBackgroundService));
            }
            finally
            {
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }

    private async Task RemoveOldDatabaseAsync(CancellationToken cancellationToken = default)
    {
        var latestDatabaseFilename = await _dmrFtpService.GetLatestDmrDatabaseAsync(cancellationToken);
        
        var fileEntries = Directory.GetFiles(_options.DestinationPath);

        foreach (var fileEntry in fileEntries)
        {
            if (fileEntry != latestDatabaseFilename)
                File.Delete(fileEntry);
        }
    }
}