using System.Collections;
using System.Windows;
using HLab.UI;

namespace HLab.Ui.Wpf;

public class UiWpfImplementation : IUiPlatformImplementation
{
    public static void Initialize()
    {
        UiPlatform.Configure(new UiWpfImplementation());
    }

    public IOpenFileDialog CreateOpenFileDialog()
    {
        throw new NotImplementedException();
    }

    public ISaveFileDialog CreateSaveFileDialog()
    {
        throw new NotImplementedException();
    }

    public IEnumerable GetLogicalChildren(object fe)
    {
        throw new NotImplementedException();
    }

    public async Task InvokeOnUiThreadAsync(Action callback)
    {
        await Application.Current.Dispatcher.InvokeAsync(callback);
    }

    public IGuiTimer CreateGuiTimer() => new GuiTimer();
    public string GetClipboardText()
    {
        return !Clipboard.ContainsText(TextDataFormat.Text) 
            ? "" : Clipboard.GetText(TextDataFormat.Text);
    }

    public void SetClipboardText(string text)
    {
        Clipboard.SetText(text);
    }
}
