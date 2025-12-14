namespace TestApp.Infrastructure.Repositories;

public interface IDirectoryRepository
{
    void TryCreate(string directoryPath);
}