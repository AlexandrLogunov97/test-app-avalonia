using System.Collections.ObjectModel;
using TestApp.Application.Models;

namespace TestApp.Application.Services;

public class SignalService : ISignalService
{
    private readonly IShapesService _shapesService;

    private readonly IDataTypesService _dataTypesService;

    private ObservableCollection<Signal> _signals = [];

    public SignalService(IShapesService shapesService, IDataTypesService dataTypesService)
    {
        _shapesService = shapesService;
        _dataTypesService = dataTypesService;
    }

    public ObservableCollection<Signal> GetSignals() => _signals;

    public void ResetSignals(IEnumerable<Signal> signals)
    {
        _signals.Clear();
        foreach (var signal in signals)
        {
            _signals.Add(signal);
        }
    }

    public void AddSignal(Signal signal)
    {
        _signals.Add(signal);
    }

    public void GenerateRandomSignals(int count)
    {
        var random = new Random();
        var shapes = _shapesService.GetShapes().ToArray();
        var dataTypes = _dataTypesService.GetDataTypes().ToArray();

        for (int i = 0; i < count; i++)
        {
            var signal = new Signal
            {
                Name = $"Signal_{_signals.Count + 1}",
                Shape = shapes[random.Next(shapes.Length)],
                Value = Math.Round(random.NextDouble() * 100, 2),
                DataType = dataTypes[random.Next(dataTypes.Length)]
            };
            AddSignal(signal);
        }
    }

    public void ClearSignals()
    {
        _signals.Clear();
    }
}