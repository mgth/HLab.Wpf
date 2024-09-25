using System.Resources;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;
using Color = System.Windows.Media.Color;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderXamlFromResource(ResourceManager resourceManager, string name, Color? foreColor)
    : IconProviderXamlParser
{
    readonly Color? _foreColor = foreColor;

    protected override object? ParseIcon(uint foregroundColor = 0)
    {
        using var xamlStream = resourceManager.GetStream(name);
        return xamlStream is null ? null : XamlTools.FromXamlStream(xamlStream);
    }

    protected override async Task<object?> ParseIconAsync(uint foregroundColor = 0)
    {
        await using var xamlStream = resourceManager.GetStream(name);
        if (xamlStream == null) return null;
        return await XamlTools.FromXamlStreamAsync(xamlStream).ConfigureAwait(true);
    }

}