using TestApp.Application.Models;
using TestApp.Infrastructure.Enums;

namespace TestApp.Infrastructure.Services;

public interface ISignalsStorageService
{
    Task<IEnumerable<Signal>> OpenAsync(string parameter, StorageType storageType);

    Task SaveAsync(IEnumerable<Signal> signals, string parameter, StorageType storageType);
}