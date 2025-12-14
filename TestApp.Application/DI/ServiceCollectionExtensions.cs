using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Application.Services;

namespace TestApp.Application.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISignalService, SignalService>();
        services.AddSingleton<IShapesService, ShapesService>();
        services.AddSingleton<IDataTypesService, DataTypesService>();

        return services;
    }
}