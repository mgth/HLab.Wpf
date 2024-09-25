using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HLab.Icons.Wpf.Icons.Providers;

public abstract class IconProviderXaml(string sourceXaml) : IconProvider
{
    string _sourceXaml = sourceXaml;

    protected override object? GetIcon(uint foregroundColor = 0) => XamlTools.FromXamlString(_sourceXaml);

    protected override async Task<object?> GetIconAsync(uint foregroundColor = 0) => await XamlTools.FromXamlStringAsync(_sourceXaml).ConfigureAwait(true);

    public override Task<string> GetTemplateAsync(uint foregroundColor = 0)
    {
        return Task.FromResult(_sourceXaml);
    }

    protected void SetSource(string  source) => _sourceXaml = source;
}

public abstract class IconProviderXamlParser() : IconProviderXaml("")
{
    bool _parsed = false;

    protected abstract object? ParseIcon(uint foregroundColor = 0);
    protected abstract Task<object?> ParseIconAsync(uint foregroundColor = 0);
        
    protected override object? GetIcon(uint foregroundColor = 0)
    {
        if (_parsed) return base.GetIcon(foregroundColor);

        if (ParseIcon(foregroundColor) is not { } icon) return null;

        SetSource(XamlWriter.Save(icon));
        _parsed = true;

        return icon;
    }

    protected override async Task<object?> GetIconAsync(uint foregroundColor = 0)
    {
        if (_parsed) return await base.GetIconAsync(foregroundColor);

        if (await ParseIconAsync(foregroundColor) is not { } icon) return null;

        await Application.Current.Dispatcher.InvokeAsync(
            ()=>SetSource(XamlWriter.Save(icon)),XamlTools.Priority2);
        _parsed = true;

        return icon;
    }

    public override async Task<string> GetTemplateAsync(uint foregroundColor = 0)
    {
        while (!_parsed)
        {
            await GetIconAsync(foregroundColor);
        }
        return await base.GetTemplateAsync(foregroundColor);
    }

}