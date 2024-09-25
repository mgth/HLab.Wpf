using System;
using System.Threading.Tasks;
using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderXamlFromUri(Uri uri) : IconProviderXamlParser
{
    protected override object? ParseIcon(uint foregroundColor = 0)
    {
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        return Application.LoadComponent(uri);;
    }
        
    protected override async Task<object?> ParseIconAsync(uint foregroundColor = 0)
    {
        object? icon = null;
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        await Application.Current.Dispatcher.InvokeAsync(
            () => icon = Application.LoadComponent(uri)
            ,XamlTools.Priority
        );

        return icon;
    }

}