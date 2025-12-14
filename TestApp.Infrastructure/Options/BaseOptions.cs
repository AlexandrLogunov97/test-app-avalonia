namespace TestApp.Infrastructure.Options;

public abstract class BaseOptions
{
    private string _path = string.Empty;

    public string DirectoryPath
    {
        get => _path;
        set => _path = value.TrimStart('/');
    }
}