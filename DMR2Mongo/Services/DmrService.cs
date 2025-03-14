using DMR.NET.Entities.Models;
using DMR2Mongo.Repositories;

namespace DMR2Mongo.Services;

public class DmrService : IDmrService
{
    private readonly IDmrRepository _repository;

    public DmrService(IDmrRepository repository)
    {
        _repository = repository;
    }

    public async Task InsertOrUpdateManyAsync(IReadOnlyList<DmrEntry> entries, CancellationToken cancellationToken = default)
    {
        await _repository.InsertOrUpdateManyAsync(entries, cancellationToken);
    }
}