using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using HLab.Base;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf;

public class MvvmWpfImpl : IMvvmPlatformImpl
{
    readonly ResourceDictionary _dictionary = new();

    public void Register(IMvvmService mvvm)
    {
        Application.Current.Resources.MergedDictionaries.Add(_dictionary);
        mvvm.ViewHelperFactory.Register<IView>(v => new ViewHelperWpf((FrameworkElement)v));
    }

    public async Task PrepareViewAsync(IView view, CancellationToken token = default)
    {
        if (view is not DependencyObject obj) return;

        ViewLocator.SetViewClass(obj, typeof(IDefaultViewClass));
        ViewLocator.SetViewMode(obj, typeof(DefaultViewMode));
    }

    static readonly object Template = XamlReader.Parse(@$"
                <DataTemplate 
                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                        <{XamlTool.Type<ViewLocator>(out var ns1)} Model=""{{Binding}}""/>
                </DataTemplate>");

    public void Register(Type type)
    {
        if (type.IsInterface) return;

        _dictionary.Add(new DataTemplateKey(type), Template);
    }

    public async Task<IView> GetNotFoundViewAsync(Type getType, Type viewMode, Type viewClass, CancellationToken token = default)
    {
        return new NotFoundView
        {
            Title = { Content = "View not found" },
            Message = { Content = (getType?.ToString() ?? "??")
                                  + "\n" + (viewMode?.FullName ?? "??")
                                  + "\n" + (viewClass?.FullName ?? "??") }
        };
    }


    public object Activate(IView obj)
    {
        throw new NotImplementedException();
    }

    public object Deactivate(IView obj)
    {
        throw new NotImplementedException();
    }

    public IWindow ViewAsWindow(IView? view)
    {
        switch (view)
        {
            case IWindow win:
                return win;
            case FrameworkElement fe:
            {
                var w = new DefaultWindow
                {
                    DataContext = fe?.DataContext,
                    View = view,
                    //SizeToContent = SizeToContent.WidthAndHeight,
                    //WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

                return w;
            }
            default:
                throw new ArgumentException("view should be FrameworkElement");
        }
    }
}