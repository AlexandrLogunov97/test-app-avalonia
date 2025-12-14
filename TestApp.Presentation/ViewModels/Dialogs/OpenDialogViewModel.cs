using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using TestApp.Infrastructure.Enums;

namespace TestApp.Presentation.ViewModels.Dialogs;

public class OpenDialogViewModel : ViewModelBase
{
    private readonly DialogViewModel _dialog;

    public ReactiveCommand<string, Unit> SelectCommand { get; }

    public OpenDialogViewModel(DialogViewModel dialog)
    {
        _dialog = dialog;

        SelectCommand = ReactiveCommand.CreateFromTask(new Func<string, Task>(Select));
    }

    private async Task Select(string storageType)
    {
        if (Enum.TryParse(typeof(StorageType), storageType, out var type))
        {
            await _dialog.CloseAsync(type);
        }
    }
}