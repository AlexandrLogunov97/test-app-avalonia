using TestApp.Application.Models;

namespace TestApp.Infrastructure.Repositories;

public interface ISignalsRepository
{
    Task<IEnumerable<Signal>> OpenAsync(string parameter);

    Task SaveAsync(IEnumerable<Signal> signals, string parameter);
}