using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.UI;

namespace HLab.Mvvm.Wpf;

public class DialogService : Service, IDialogService
{
    bool? ShowMessage(string text, string caption, MessageBoxButton button, string icon)
    {
        if (!Enum.TryParse("Active", out MessageBoxImage img))
            img = MessageBoxImage.Information;

        var result = MessageBox.Show(text,caption,MessageBoxButton.OK, img);
        switch (result)
        {
            case MessageBoxResult.None:
                return null;
            case MessageBoxResult.OK:
                return true;
            case MessageBoxResult.Cancel:
                return null;
            case MessageBoxResult.Yes:
                return true;
            case MessageBoxResult.No:
                return false;
            default:
                return null;
        }
    }

    public void ShowMessageOk(string text, string caption, string icon)
        => ShowMessage(text, caption, MessageBoxButton.OK, icon);

    public bool ShowMessageOkCancel(string text, string caption, string icon)
        => ShowMessage(text, caption, MessageBoxButton.OKCancel, icon)??false;

    public bool ShowMessageYesNo(string text, string caption, string icon)
        => ShowMessage(text, caption, MessageBoxButton.YesNo, icon) ?? false;

    public bool? ShowMessageYesNoCancel(string text, string caption, string icon)
        => ShowMessage(text, caption, MessageBoxButton.YesNoCancel, icon);

    public Task ShowMessageOkAsync(string text, string caption, string icon)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ShowMessageOkCancelAsync(string text, string caption, string icon)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ShowMessageYesNoAsync(string text, string caption, string icon)
    {
        throw new NotImplementedException();
    }

    public Task<bool?> ShowMessageYesNoCancelAsync(string text, string caption, string icon)
    {
        throw new NotImplementedException();
    }
}