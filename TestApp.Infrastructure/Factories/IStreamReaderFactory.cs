namespace TestApp.Infrastructure.Factories;

public interface IStreamReaderFactory
{
    StreamReader CreateReader(string path);
}