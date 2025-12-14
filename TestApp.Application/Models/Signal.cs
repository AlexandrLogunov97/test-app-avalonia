namespace TestApp.Application.Models;

public class Signal
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Shape { get; set; } = string.Empty;

    public double Value { get; set; }

    public string DataType { get; set; } = string.Empty;

    public Signal() { }

    public Signal(string name, string shape, double value, string dataType)
    {
        Name = name;
        Shape = shape;
        Value = value;
        DataType = dataType;
    }
}
