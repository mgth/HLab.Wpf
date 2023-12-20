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

    public object Get(uint foregroundColor = 0)
    {
        throw new System.NotImplementedException();
    }

    public Task<object> GetAsync(uint foregroundColor = 0)
    {
        throw new System.NotImplementedException();
    }

    public Task<string> GetTemplateAsync(uint foregroundColor = 0)
    {
        throw new System.NotImplementedException();
    }
}