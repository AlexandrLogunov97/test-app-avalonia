namespace TestApp.Infrastructure.Factories;

public interface IStreamWriterFactory
{
    StreamWriter CreateWriter(string path);
}