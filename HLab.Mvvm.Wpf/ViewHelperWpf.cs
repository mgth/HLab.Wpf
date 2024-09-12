using System.Windows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf;

namespace HLab.Mvvm.Wpf;

public class ViewHelperWpf(FrameworkElement view) : IViewHelper
{
    public FrameworkElement View { get; } = view;

        public FrameworkElement View { get; }
        public IMvvmContext Context
        {
            get => (IMvvmContext) View.GetValue(ViewLocator.MvvmContextProperty);
            set => View.SetValue(ViewLocator.MvvmContextProperty, value);
        }

        public object Linked
        {
            get => View.DataContext;
            set => View.DataContext = value;
        }
    }
}
