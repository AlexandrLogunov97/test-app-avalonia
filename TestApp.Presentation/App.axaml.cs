using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TestApp.Application.DI;
using TestApp.Infrastructure.DI;
using TestApp.Presentation.DI;
using TestApp.Presentation.ViewModels;
using TestApp.Presentation.Views;

namespace TestApp.Presentation;

public partial class App : Avalonia.Application
{
    private IServiceProvider _serviceProvider = null!;

    private IConfiguration _configuration = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);
        services.AddInfrastructure(_configuration);
        services.AddApplication(_configuration);
        services.AddPresentation(_configuration);
    }
}