using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media;
using HLab.Core.Annotations;
using HLab.Icons.Wpf.Icons.Providers;
using HLab.Mvvm.Annotations;

namespace HLab.Icons.Wpf.Icons;

public class IconBootloader(IIconService icons) : IBootloader
{
   public async Task LoadAsync(IBootContext bootstrapper)
   {
      Load();
   }

   public void Load()
   {
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(e => !e.IsDynamic))
      {
         try
         {
            var v = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            if (v?.Company == "Microsoft Corporation") continue;

            var resourceManager = new ResourceManager(assembly.GetName().Name + ".g", assembly);
            var resources = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (var rkey in resources)
            {
               var r = ((DictionaryEntry)rkey).Key.ToString().ToLower();

               var resourcePath = r.Replace(assembly.ManifestModule.Name.Replace(".exe", "") + ".", "");

               resourcePath = Uri.UnescapeDataString(resourcePath);

               if (HasSuffix(resourcePath, ".xaml", out var xamlPath))
               {
                  icons.AddIconProvider(xamlPath, new IconProviderXamlFromResource(resourceManager, resourcePath, Colors.Black));
               }
               else if (HasSuffix(resourcePath, ".svg", out var svgPath))
               {
                  icons.AddIconProvider(svgPath, new IconProviderSvg(resourceManager, resourcePath, Colors.Black));
               }
               else if (HasSuffix(resourcePath, ".baml", out var bamlPath))
               {
                  icons.AddIconProvider(
                      bamlPath,
                      new IconProviderXamlFromUri(new Uri($"/{assembly.FullName};component/{bamlPath}.xaml", UriKind.Relative))
                  );
               }

               continue;

               bool HasSuffix(string path, string suffix, out string result)
               {
                  if (!path.EndsWith(suffix))
                  {
                     result = null;
                     return false;
                  }

                  result = path[..^suffix.Length];
                  return true;
               }
            }
         }
         catch (MissingManifestResourceException ex)
         {
         }


      }
   }
}