using ReactiveUI;

namespace TestApp.Presentation.Models;

public class SignalModel : ReactiveObject
{
    private string _name = string.Empty;

    private string _shape = string.Empty;

    private double _value;

    private string _dataType = string.Empty;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Shape
    {
        get => _shape;
        set => this.RaiseAndSetIfChanged(ref _shape, value);
    }

    public double Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public string DataType
    {
        get => _dataType;
        set => this.RaiseAndSetIfChanged(ref _dataType, value);
    }

    public SignalModel() { }

    public SignalModel(string name, string shape, double value, string dataType)
    {
        Name = name;
        Shape = shape;
        Value = value;
        DataType = dataType;
    }

    public SignalModel Clone() => new()
    {
        Name = Name,
        Shape = Shape,
        Value = Value,
        DataType = DataType
    };
}