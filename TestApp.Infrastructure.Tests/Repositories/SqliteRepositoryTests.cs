using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moq;
using TestApp.Application.Models;
using TestApp.Infrastructure.Options;
using TestApp.Infrastructure.Repositories;

namespace TestApp.Infrastructure.Tests.Repositories;

public class SqliteRepositoryTests : IDisposable
{
    private SqliteConnection _connection;

    private readonly string _connectionString = "DataSource=:memory:";

    public SqliteRepositoryTests()
    {
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }

    [Fact]
    public async void OpenAsync_ShouldOpenAndLoadSignbals_IfTheyExist()
    {
        // Arrange
        var options = new SqliteOptions
        {
            ConnectionString = _connectionString
        };

        var mockOptions = new Mock<IOptions<SqliteOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockRepository = new Mock<IDirectoryRepository>();
        mockRepository.Setup(x => x.TryCreate(It.IsAny<string>()));

        var sqliteRepository = new SqliteRepository(mockOptions.Object, _connection);

        await CreateTableAsync();

        var testData = new Signal[]
        {
            new Signal("Test 1", "Test Shape", 1, "Boolean"),
            new Signal("Test 2", "Test Shape", 0, "Boolean")
        };

        await FillDbAsync(testData);

        // Act
        var result = await sqliteRepository.OpenAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal(testData[0].Name, result.ElementAt(0).Name);
        Assert.Equal(testData[0].Shape, result.ElementAt(0).Shape);
        Assert.Equal(testData[0].Value, result.ElementAt(0).Value);
        Assert.Equal(testData[0].DataType, result.ElementAt(0).DataType);

        await ClearDbAsync();
    }

    [Fact]
    public async void SaveAsync_ShouldSaveSignals_IfDataSuccessfulySaved()
    {
        // Arrange
        var options = new SqliteOptions
        {
            ConnectionString = _connectionString
        };

        var mockOptions = new Mock<IOptions<SqliteOptions>>();
        mockOptions.Setup(x => x.Value).Returns(options);

        var mockRepository = new Mock<IDirectoryRepository>();
        mockRepository.Setup(x => x.TryCreate(It.IsAny<string>()));

        var sqliteRepository = new SqliteRepository(mockOptions.Object, _connection);

        await CreateTableAsync();

        var testData = new Signal[]
        {
            new Signal("Test 1", "Test Shape", 1, "Boolean"),
            new Signal("Test 2", "Test Shape", 0, "Boolean")
        };

        // Act
        await sqliteRepository.SaveAsync(testData);

        // Assert
        var assertData = await GetDbDataAsync();
        Assert.Equal(2, assertData.Count());
        Assert.Equal(testData[0].Name, assertData.ElementAt(0).Name);
        Assert.Equal(testData[0].Shape, assertData.ElementAt(0).Shape);
        Assert.Equal(testData[0].Value, assertData.ElementAt(0).Value);
        Assert.Equal(testData[0].DataType, assertData.ElementAt(0).DataType);

        await ClearDbAsync();
    }

    public async Task<IEnumerable<Signal>> GetDbDataAsync()
    {
        var signals = new List<Signal>();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM Signals";

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var signal = new Signal
                {
                    Id = Guid.Parse(reader.GetString(0)),
                    Name = reader.GetString(1),
                    Shape = reader.GetString(2),
                    Value = reader.GetDouble(3),
                    DataType = reader.GetString(4)
                };
                signals.Add(signal);
            }
        }

        return signals;
    }

    private async Task ClearDbAsync()
    {
        var clearCommand = _connection.CreateCommand();
        clearCommand.CommandText = "DELETE FROM Signals";
        await clearCommand.ExecuteNonQueryAsync();
    }

    private async Task FillDbAsync(IEnumerable<Signal> signals)
    {
        foreach (var signal in signals)
        {
            var insertCommand = _connection.CreateCommand();
            insertCommand.CommandText = @"
                        INSERT INTO Signals (Id, Name, Shape, Value, DataType)
                        VALUES (@Id, @Name, @Shape, @Value, @DataType)";

            insertCommand.Parameters.AddWithValue("@Id", signal.Id.ToString());
            insertCommand.Parameters.AddWithValue("@Name", signal.Name);
            insertCommand.Parameters.AddWithValue("@Shape", signal.Shape);
            insertCommand.Parameters.AddWithValue("@Value", signal.Value);
            insertCommand.Parameters.AddWithValue("@DataType", signal.DataType);

            await insertCommand.ExecuteNonQueryAsync();
        }
    }

    private async Task CreateTableAsync()
    {
        var createTableCommand = _connection.CreateCommand();
        createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Signals (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Shape TEXT NOT NULL,
                    Value REAL NOT NULL,
                    DataType TEXT NOT NULL
                )";

        await createTableCommand.ExecuteNonQueryAsync();
    }
}