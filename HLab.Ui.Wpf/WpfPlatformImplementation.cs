using HLab.UI;
using System.Collections;
using System.Windows.Threading;

namespace HLab.Ui.Wpf;

public class UiWpfImplementation : IUiPlatformImplementation
{
    public static void Initialize()
    {
        UiPlatform.Implementation = new UiWpfImplementation();
    }

    public IOpenFileDialog CreateOpenFileDialog() => throw new NotImplementedException();

    public ISaveFileDialog CreateSaveFileDialog() => throw new NotImplementedException();

    public IEnumerable GetLogicalChildren(object fe) => throw new NotImplementedException();

    public async Task InvokeOnUiThreadAsync(Action callback)
    {
        await Dispatcher.CurrentDispatcher.InvokeAsync(callback);
    }
}
