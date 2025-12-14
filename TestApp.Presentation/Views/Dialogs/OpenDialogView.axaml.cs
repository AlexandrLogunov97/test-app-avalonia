using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TestApp.Presentation.Views.Dialogs;

public partial class OpenDialogView : UserControl
{
    public OpenDialogView()
    {
        InitializeComponent();
    }

    private void Button_ActualThemeVariantChanged(object? sender, System.EventArgs e)
    {
    }
}