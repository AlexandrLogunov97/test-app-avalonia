namespace TestApp.Infrastructure.Factories;

public class FileStreamFactory : IFileStreamFactory
{
    public StreamReader CreateReader(string path)
    {
        return new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read));
    }

    public StreamWriter CreateWriter(string path)
    {
        return new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
    }
}