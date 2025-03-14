using DMR.NET.Entities.Models;

namespace DMR2Mongo.Services;

public interface IDmrService
{
    Task InsertOrUpdateManyAsync(IReadOnlyList<DmrEntry> entries, CancellationToken cancellationToken = default);
}