using DMR.NET.Entities.Models;

namespace DMR2Mongo.Repositories;

public interface IDmrRepository
{
    Task InsertOrUpdateManyAsync(IReadOnlyList<DmrEntry> entries, CancellationToken cancellationToken = default);
}