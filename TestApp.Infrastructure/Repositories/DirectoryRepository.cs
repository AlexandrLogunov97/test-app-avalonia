namespace TestApp.Infrastructure.Repositories;

public class DirectoryRepository : IDirectoryRepository
{
    public void TryCreate(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}