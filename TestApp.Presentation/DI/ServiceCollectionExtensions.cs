using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Presentation.ViewModels;
using TestApp.Presentation.ViewModels.Dialogs;
using TestApp.Presentation.Views;


namespace TestApp.Presentation.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        // View models
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<EditorViewModel>();
        services.AddTransient<ChartViewModel>();
        services.AddTransient<OpenDialogViewModel>();
        services.AddTransient<SaveDialogViewModel>();
        services.AddSingleton<DialogViewModel>();

        // Views
        services.AddTransient<MainWindow>();

        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);

        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}