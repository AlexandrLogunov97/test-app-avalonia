using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TestApp.Infrastructure.Factories;
using TestApp.Infrastructure.Options;
using TestApp.Infrastructure.Repositories;
using TestApp.Infrastructure.Services;

namespace TestApp.Infrastructure.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonOptions>(configuration.GetSection("JsonOptions"));
        services.Configure<XmlOptions>(configuration.GetSection("XmlOptions"));
        services.Configure<SqliteOptions>(configuration.GetSection("SqliteOptions"));


        services.AddTransient<IDirectoryRepository, DirectoryRepository>();
        services.AddTransient<IFileStreamFactory, FileStreamFactory>();

        services.AddJsonRepository();
        services.AddXmlRepository();
        services.AddSqliteRepository();

        services.AddSingleton<ISignalsStorageService, SignalsStorageService>();

        return services;
    }

    private static IServiceCollection AddJsonRepository(this IServiceCollection services)
    {
        services.AddSingleton<JsonRepository>(sp => 
        {
            var options = sp.GetService<IOptions<JsonOptions>>()!;
            var fileFactory = sp.GetService<IFileStreamFactory>()!;
            var directoryRepository = sp.GetService<IDirectoryRepository>()!;
            directoryRepository.TryCreate(options.Value.DirectoryPath);

            return new JsonRepository(options, fileFactory);
        });

        return services;
    }

    private static IServiceCollection AddXmlRepository(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var options = sp.GetService<IOptions<XmlOptions>>()!;
            var fileFactory = sp.GetService<IFileStreamFactory>()!;
            var directoryRepository = sp.GetService<IDirectoryRepository>()!;
            directoryRepository.TryCreate(options.Value.DirectoryPath);

            return new XmlRepository(options, fileFactory);
        });

        return services;
    }

    private static IServiceCollection AddSqliteRepository(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            SQLitePCL.Batteries.Init();

            var options = sp.GetService<IOptions<SqliteOptions>>()!;
            var directoryRepository = sp.GetService<IDirectoryRepository>()!;
            directoryRepository.TryCreate(options.Value.DirectoryPath);

            var connection = new SqliteConnection(options.Value.ConnectionString);

            connection.Open();

            var createTableCommand = connection.CreateCommand();
            createTableCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS Signals (
                    Id TEXT PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Shape TEXT NOT NULL,
                    Value REAL NOT NULL,
                    DataType TEXT NOT NULL
                )";

            createTableCommand.ExecuteNonQuery();
            var repository = new SqliteRepository(options, connection);

            return repository;
        });



        return services;
    }
}