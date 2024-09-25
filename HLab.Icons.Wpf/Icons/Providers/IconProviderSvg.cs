using System;
using System.IO;
using System.Resources;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HLab.Mvvm.Annotations;
using Svg;
using Image = System.Windows.Controls.Image;

namespace HLab.Icons.Wpf.Icons.Providers;

public class IconProviderSvg(ResourceManager resourceManager, string name, Color? foreColor)
    : IconProviderXamlParser
{
    protected override async Task<object?> ParseIconAsync(uint foregroundColor = 0)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        await using var svg = resourceManager.GetStream(name);
        if(svg is null) return null;
        return await XamlTools.FromSvgStreamAsync(svg).ConfigureAwait(true);
    }

    protected override object? ParseIcon(uint foregroundColor = 0)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        using var svg = resourceManager.GetStream(name);
        return svg is null ? null : XamlTools.FromSvgStream(svg);
    }


    public async Task<object> GetBitmapAsync()
    {
        AppContext.SetSwitch("Switch.System.Xml.AllowDefaultResolver", true);
        await using var svg = resourceManager.GetStream(name);
//            var icon = await XamlTools.FromSvgStreamAsync(svg).ConfigureAwait(true);
        var icon = SvgDocument.Open<SvgDocument>(svg);

        var b = new System.Drawing.Bitmap(128, 128);
        b.MakeTransparent();
        icon.Draw(b);
        return new Image {Source = Convert(b)};
    }

    BitmapImage Convert(System.Drawing.Bitmap src)
    {
        MemoryStream ms = new MemoryStream();
        ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        BitmapImage image = new BitmapImage();
        image.BeginInit();
        ms.Seek(0, SeekOrigin.Begin);
        image.StreamSource = ms;
        image.EndInit();
        return image;
    }

}