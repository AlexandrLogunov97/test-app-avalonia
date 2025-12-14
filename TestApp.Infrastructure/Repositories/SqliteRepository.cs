using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TestApp.Application.Models;
using TestApp.Infrastructure.Options;

namespace TestApp.Infrastructure.Repositories;

public class SqliteRepository : ISignalsRepository, IDisposable
{
    private SqliteConnection _connection = null!;

    private readonly SqliteOptions _options;

    public SqliteRepository(IOptions<SqliteOptions> options, SqliteConnection connection)
    {
        _options = options.Value;
        _connection = connection;
    }

    public async Task<IEnumerable<Signal>> OpenAsync(string parameter = "signals")
    {
        var signals = new List<Signal>();

        var command = _connection.CreateCommand();
        command.CommandText = "SELECT * FROM Signals";

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var signal = MapToSignal(reader);
                signals.Add(signal);
            }
        }

        return signals;
    }

    public async Task SaveAsync(IEnumerable<Signal> signals, string parameter = "signals")
    {
        using var transaction = _connection.BeginTransaction();

        try
        {
            var clearCommand = _connection.CreateCommand();
            clearCommand.CommandText = "DELETE FROM Signals";
            await clearCommand.ExecuteNonQueryAsync();

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

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public void Dispose()
    {
        _connection?.Close();
        _connection?.Dispose();
    }

    private Signal MapToSignal(SqliteDataReader reader)
    {
        return new Signal
        {
            Id = Guid.Parse(reader.GetString(0)),
            Name = reader.GetString(1),
            Shape = reader.GetString(2),
            Value = reader.GetDouble(3),
            DataType = reader.GetString(4)
        };
    }
}