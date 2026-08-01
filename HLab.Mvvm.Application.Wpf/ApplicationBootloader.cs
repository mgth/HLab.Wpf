using System;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using HLab.Core.Annotations;
using HLab.Erp.Acl.LoginServices;
using HLab.Erp.Core.Update;
using HLab.Erp.Core.Wpf.Localization;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Menus;
using HLab.Mvvm.Application.Updater;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Mvvm.Application.Wpf
{
    public class ApplicationBootloader : Bootloader
    {
        readonly IMenuService _menu;
        readonly IMvvmService _mvvm;
        readonly IApplicationInfoService _info;

        public IUpdater Updater { get; set; }
        readonly Func<MainWpfViewModel> _getMainViewModel;

        public Func<ProgressLoadingViewModel> GetProgressLoadingViewModel { get; set; }

        public ApplicationBootloader(IMenuService menu, IMvvmService mvvm, IApplicationInfoService info, Func<MainWpfViewModel> getMainViewModel)
        {
            _menu = menu;
            _mvvm = mvvm;
            _info = info;
            _getMainViewModel = getMainViewModel;
        }

        public void SetMainViewMode(Type vm)
        {
            MainViewMode = vm;
        }

        // DefaultViewMode par défaut : personne n'appelle SetMainViewMode, et la
        // résolution de vues modernisée ne tolère plus un viewMode null.
        public Type MainViewMode { get; private set; } = typeof(DefaultViewMode);

        public MainWpfViewModel? ViewModel { get; set; } 
        public IWindow MainWindow { get; protected set; }




        public override async Task<BootState> LoadAsync()
        {
            if (WaitingForBootloader<LocalizeFromDb.Bootloader>() || WaitingForBootloader<LoginBootloader>()) return BootState.Requeue;

            _info.Version = Assembly.GetEntryAssembly()?.GetName().Version;

            //  InitializeCultures();

            if (Updater != null )
            {
                Updater.CheckVersion();

                if (Updater.NewVersionFound)
                {
                    var updaterView = new ApplicationUpdateView
                    {
                        DataContext = Updater
                    };
                    // TODO : updaterView.ShowDialog();

                    if (Updater.Updated)
                    {
                        System.Windows.Application.Current.Shutdown();
                        return BootState.Cancel;
                    }
                }
            }

            ViewModel = _getMainViewModel();

            MainWindow = _mvvm.ViewAsWindow(_mvvm.MainContext.GetView(ViewModel,MainViewMode,typeof(IDefaultViewClass)));

// TODO URGENT
            // MainWindow.Closing += (sender, args) => System.Windows.Application.Current.Shutdown();

            _menu.RegisterMenu("file", "{File}", null, null);
            _menu.RegisterMenu("data", "{Data}", null, null);
            _menu.RegisterMenu("param", "{Parameters}", null, null);
            _menu.RegisterMenu("tools", "{Tools}", null, null);
            _menu.RegisterMenu("help", "{_?}", null, null);


            _menu.RegisterMenu("file/exit","{Exit}", ViewModel.Exit,null);

            MainWindow.Show();

            return BootState.Completed;
        }

    }
}
