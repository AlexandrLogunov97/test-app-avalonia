namespace TestApp.Application.Services;

public class ShapesService : IShapesService
{
    private string[] _shapes = 
        [
            "Sine",
            "Square",
            "Triangle",
        ];

    public IEnumerable<string> GetShapes() => _shapes.AsReadOnly();
}