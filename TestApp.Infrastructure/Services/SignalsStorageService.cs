using TestApp.Application.Models;
using TestApp.Infrastructure.Enums;
using TestApp.Infrastructure.Repositories;

namespace TestApp.Infrastructure.Services;

public class SignalsStorageService : ISignalsStorageService
{
    private readonly Dictionary<StorageType, ISignalsRepository> _repositories;

    public SignalsStorageService(JsonRepository jsonRepository, XmlRepository xmlRepository, SqliteRepository sqlLiteRepository)
    {
        _repositories = new Dictionary<StorageType, ISignalsRepository>
        {
            { StorageType.Json, jsonRepository },
            { StorageType.Xml, xmlRepository },
            { StorageType.Sqlite, sqlLiteRepository }
        };
    }

    public async Task<IEnumerable<Signal>> OpenAsync(string parameter, StorageType storageType)
    {
        if (!_repositories.TryGetValue(storageType, out var repository)) return Enumerable.Empty<Signal>();

        var signals = await repository.OpenAsync(parameter);

        return signals;
    }

    public async Task SaveAsync(IEnumerable<Signal> signals, string parameter, StorageType storageType)
    {
        if (!_repositories.TryGetValue(storageType, out var repository)) return;

        await repository.SaveAsync(signals, parameter);
    }
}