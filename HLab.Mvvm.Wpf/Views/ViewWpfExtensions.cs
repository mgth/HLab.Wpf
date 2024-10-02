using System;
using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Views
{
    public static class ViewWpfExtensions
    {

        public static TViewModel? ViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view)
            where TViewMode : ViewMode =>
            view is not FrameworkElement fe ? default : fe.DataContext is not TViewModel vm ? default : vm;

        public static bool TryGetViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view,out TViewModel? viewModel)
            where TViewMode : ViewMode
        {
            if (view is FrameworkElement { DataContext: TViewModel vm })
            {
                viewModel = vm;
                return true;
            }

            viewModel = default;
            return false;
        }
    }
}
