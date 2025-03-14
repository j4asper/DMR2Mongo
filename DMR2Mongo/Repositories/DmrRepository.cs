using DMR.NET.Entities.Models;
using MongoDB.Driver;

namespace DMR2Mongo.Repositories;

public class DmrRepository : IDmrRepository
{
    private const string CollectionName = "Entries";
    private readonly IMongoCollection<DmrEntry> _collection;

    public DmrRepository(IMongoDatabase mongoDatabase)
    {
        _collection = mongoDatabase.GetCollection<DmrEntry>(CollectionName);
    }

    public async Task InsertOrUpdateManyAsync(IReadOnlyList<DmrEntry> entries, CancellationToken cancellationToken = default)
    {
        var bulkOps = entries.Select(entry => new ReplaceOneModel<DmrEntry>(
            Builders<DmrEntry>.Filter.Eq(e => e.Id, entry.Id),
            entry
        )
        {
            IsUpsert = true
        }).ToList();

        await _collection.BulkWriteAsync(bulkOps, cancellationToken: cancellationToken);
    }
}