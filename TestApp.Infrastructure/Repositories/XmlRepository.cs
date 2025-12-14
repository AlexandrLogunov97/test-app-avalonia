using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using TestApp.Application.Models;
using TestApp.Infrastructure.Factories;
using TestApp.Infrastructure.Options;

namespace TestApp.Infrastructure.Repositories;

public class XmlRepository : ISignalsRepository
{
    private readonly XmlSerializer _serializer;

    private readonly IFileStreamFactory _factory;

    private XmlOptions _options;

    public XmlRepository(IOptions<XmlOptions> options, IFileStreamFactory factory)
    {
        _options = options.Value;
        _factory = factory;
        _serializer = new XmlSerializer(typeof(List<Signal>), new XmlRootAttribute("Signals"));
    }

    public async Task<IEnumerable<Signal>> OpenAsync(string parameter = "signals")
    {
        try
        {
            var file = $"{_options.DirectoryPath}/{parameter}.xml";
            using var reader = _factory.CreateReader(file);
            var signals = _serializer.Deserialize(reader) as List<Signal>;

            return signals ?? Enumerable.Empty<Signal>();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error during reading XML file: {ex.Message}", ex);
        }
    }

    public async Task SaveAsync(IEnumerable<Signal> signals, string parameter = "signals")
    {
        try
        {
            var file = $"{_options.DirectoryPath}/{parameter}.xml";
            using var writer = _factory.CreateWriter(file);
            _serializer.Serialize(writer, new List<Signal>(signals));

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error durig saving XML file: {ex.Message}", ex);
        }
    }
}