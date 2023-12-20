using System.Threading.Tasks;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderSvgFromSource : IconProviderXamlParser, IIconProvider
{
    readonly string _name;
    readonly string _source;
 
    public IconProviderSvgFromSource(string source, string name, int? foreColor)
    {
        _source = source; 
        _name = name;
    }

    protected override async Task<object?> ParseIconAsync() => await XamlTools.FromSvgStringAsync(_source);

    protected override object? ParseIcon()=> XamlTools.FromSvgString(_source);

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