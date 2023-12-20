using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using HLab.Sys.Windows.API;

namespace HLab.Base.Wpf.Extensions;

internal class BlurHelper
{
    readonly WeakReference<Window> _window;
    readonly bool _enable;

    public BlurHelper(Window window, bool enable)
    {
        _enable = enable;
        _window = new WeakReference<Window>(window);

        if (window.IsLoaded)
            Window_Loaded(null, null);
        else
            window.Loaded += Window_Loaded;

    }


    void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (_window == null || !_window.TryGetTarget(out var window)) return;

        window.Loaded -= Window_Loaded;

        var windowHelper = new WindowInteropHelper(window);

        var accent = new DwmApi.AccentPolicy();
        var accentStructSize = Marshal.SizeOf(accent);
        accent.AccentState = _enable ? DwmApi.AccentState.EnableBlurBehind : DwmApi.AccentState.Disabled; //NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;

        var accentPtr = Marshal.AllocHGlobal(accentStructSize);
        Marshal.StructureToPtr(accent, accentPtr, false);

        var data = new DwmApi.WindowCompositionAttributeData
        {
            Attribute = DwmApi.WindowCompositionAttribute.AccentPolicy,
            SizeOfData = accentStructSize,
            Data = accentPtr
        };

        _ = DwmApi.SetWindowCompositionAttribute(windowHelper.Handle, ref data);

        Marshal.FreeHGlobal(accentPtr);
        _window.SetTarget(null);
    }
}