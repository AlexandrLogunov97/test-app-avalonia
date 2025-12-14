using ReactiveUI;
using System.Reactive;
using System;
using TestApp.Infrastructure.Enums;
using System.Threading.Tasks;

namespace TestApp.Presentation.ViewModels.Dialogs;

public class SaveDialogViewModel : ViewModelBase
{
    private readonly DialogViewModel _dialog;

    public ReactiveCommand<string, Unit> SelectCommand { get; }

    public SaveDialogViewModel(DialogViewModel dialog)
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
