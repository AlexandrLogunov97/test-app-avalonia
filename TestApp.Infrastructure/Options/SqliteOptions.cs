namespace TestApp.Infrastructure.Options;

public class SqliteOptions : BaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}