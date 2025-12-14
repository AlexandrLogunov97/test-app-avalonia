using Microsoft.Extensions.Options;
using System.Text.Json;
using TestApp.Application.Models;
using TestApp.Infrastructure.Factories;
using TestApp.Infrastructure.Options;

namespace TestApp.Infrastructure.Repositories;

public class JsonRepository : ISignalsRepository
{
    private readonly JsonSerializerOptions _jsonOptions;

    private readonly IFileStreamFactory _factory;

    private JsonOptions _options;

    public JsonRepository(IOptions<JsonOptions> options, IFileStreamFactory factory)
    {
        _options = options.Value;
        _factory = factory;

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IEnumerable<Signal>> OpenAsync(string parameter = "signals")
    {
        try
        {
            var file = $"{_options.DirectoryPath}/{parameter}.json";

            using var reader = _factory.CreateReader(file);
            var json = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<Signal>();
            }

            var signals = JsonSerializer.Deserialize<List<Signal>>(json, _jsonOptions);

            return signals ?? Enumerable.Empty<Signal>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during reading JSON file: {ex.Message}", ex);
        }
    }

    public async Task SaveAsync(IEnumerable<Signal> signals, string parameter = "signals")
    {
        try
        {
            var file = $"{_options.DirectoryPath}/{parameter}.json";
            using var writer = _factory.CreateWriter(file);
            var json = JsonSerializer.Serialize(signals, _jsonOptions);
            await writer.WriteAsync(json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during saving JSON file: {ex.Message}", ex);
        }
    }
}