using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using HLab.ColorTools.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderXamlFromSource(string source, string name, int? foreground)
    : IconProviderXaml(source), IIconProvider
{
    readonly string _name = name;
    readonly int? _foreColor = foreground;
}