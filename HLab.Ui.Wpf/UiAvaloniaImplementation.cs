using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using HLab.UI;

namespace HLab.Ui.Wpf;

public class UiWpfImplementation : IUiPlatformImplementation
{        
    public static void InitializeCultures()
    {
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(
                XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));
    }

    public static void Initialize()
    {
        InitializeCultures();
        UiPlatform.Configure<UiWpfImplementation>();
    }

    public IOpenFileDialog CreateOpenFileDialog() => throw new NotImplementedException();

    public ISaveFileDialog CreateSaveFileDialog() => throw new NotImplementedException();

    public IEnumerable GetLogicalChildren(object fe) => throw new NotImplementedException();

    public Task InvokeOnUiThreadAsync(Action callback) => Application.Current.Dispatcher.InvokeAsync(callback).Task;

    public Task InvokeOnUiThreadAsync(Func<Task> callback) => Application.Current.Dispatcher.InvokeAsync(callback).Task;

    public void VerifyAccess() => Application.Current.Dispatcher.VerifyAccess();

    public IGuiTimer CreateGuiTimer() => new GuiTimer();
    public string GetClipboardText() => !Clipboard.ContainsText(TextDataFormat.Text) ? "" : Clipboard.GetText(TextDataFormat.Text);

    public void SetClipboardText(string text) => Clipboard.SetText(text);

    public void Quit() => Application.Current.Shutdown();
}
