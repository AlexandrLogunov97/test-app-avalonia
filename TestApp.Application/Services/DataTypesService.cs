namespace TestApp.Application.Services;

public class DataTypesService : IDataTypesService
{
    private string[] _dataTypes =
        [
            "Int32",
            "Boolean",
            "Double",
        ];

    public IEnumerable<string> GetDataTypes() => _dataTypes.AsReadOnly();
}