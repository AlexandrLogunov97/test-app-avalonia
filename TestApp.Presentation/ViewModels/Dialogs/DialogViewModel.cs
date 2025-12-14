using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;

namespace TestApp.Presentation.ViewModels.Dialogs;

public class DialogViewModel : ViewModelBase
{
    private readonly IServiceProvider _serviceProvider;

    private ViewLocator _locator = new();

    private Func<object?, Task>? _fallback = null;

    private bool _isOpen;

    private object? _view;

    public bool IsOpen
    {
        get => _isOpen;
        set => this.RaiseAndSetIfChanged(ref _isOpen, value);
    }

    public object? View
    {
        get => _view;
        set => this.RaiseAndSetIfChanged(ref _view, value);
    }

    public ReactiveCommand<Unit, Unit> CloseCommand { get; }

    public DialogViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        CloseCommand = ReactiveCommand.Create(CloseDialog);
    }

    public void Open<TViewModel>(Func<object?, Task>? fallback = null) where TViewModel : ViewModelBase
    {
        _fallback = fallback;
        var viewModel = _serviceProvider.GetService<TViewModel>();
        var view = _locator.Build(viewModel);
        if (view != null && viewModel != null)
        {
            view.DataContext = viewModel;
            View = view;
            IsOpen = true;
        }
    }

    public async Task CloseAsync(object? result = null)
    {
        if ( _fallback != null)
        {
            await _fallback.Invoke(result);
        }

        CloseDialog();
    }

    private void CloseDialog()
    {
        IsOpen = false;
    }
}