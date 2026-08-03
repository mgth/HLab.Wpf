using System;
using System.Windows;
using System.Windows.Interop;

namespace HLab.Mvvm.Wpf;

/// <summary>
/// Neutralise la maximisation au double-clic de légende.
///
/// Les fenêtres HLab n'ont pas de barre de titre : les CaptionHeight pixels
/// du WindowChrome recouvrent la barre de menus (et le haut du contenu). Tout
/// double-clic qui n'est pas consommé par un contrôle y est donc interprété
/// par le système comme un double-clic de légende, et bascule la fenêtre en
/// plein écran — comportement déroutant puisque rien n'indique une légende à
/// cet endroit. WindowChrome.IsHitTestVisibleInChrome ne suffit pas : WPF ne
/// teste que l'élément directement touché, pas ses ancêtres, il faudrait donc
/// le poser sur chaque feuille de la bande haute.
///
/// Le glissement de la fenêtre (drag) et les boutons système sont conservés.
/// </summary>
static class CaptionDoubleClick
{
    const int WmNcLButtonDblClk = 0x00A3;

    public static void Disable(Window window)
    {
        if (PresentationSource.FromVisual(window) is HwndSource source)
            source.AddHook(Hook);
    }

    static IntPtr Hook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WmNcLButtonDblClk) handled = true;
        return IntPtr.Zero;
    }
}
