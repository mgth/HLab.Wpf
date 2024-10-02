using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HLab.Base.ReactiveUI;
using HLab.Erp.Acl;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Mvvm.Application.Wpf;

public class MainWpfViewModel : ViewModel
    {
        public IAclService Acl {get; }
        readonly IDocumentService _doc;
        public IApplicationInfoService ApplicationInfo { get; }
        public ILocalizationService LocalizationService { get; }
        public IIconService IconService { get; }

        public MainWpfViewModel(
            IAclService acl, 
            IDocumentService doc, 
            IDocumentPresenter presenter, 
            IApplicationInfoService applicationInfo, 
            ILocalizationService localizationService, 
            IIconService iconService)
        {
            Acl = acl;
            DocumentPresenter = presenter;
            doc.MainPresenter = presenter;
            _doc = doc;
            ApplicationInfo = applicationInfo;
            LocalizationService = localizationService;
            IconService = iconService;

        _title = this
            .WhenAnyValue(e => e.ApplicationInfo.Name)
            .ToProperty(this, e => e.Title);

        Exit = ReactiveCommand.Create(() => System.Windows.Application.Current.Shutdown());

        OpenUserCommand = ReactiveCommand.CreateFromTask(() => doc.OpenDocumentAsync(Acl.Connection.User));

        }

        public IDocumentPresenter DocumentPresenter { get; }

        public bool IsActive
        {
        get => _isActive;
        set => this.SetAndRaise(ref _isActive,value);
        }
    bool _isActive = true;




        // TODO
        //public Canvas DragCanvas => _dragCanvas.Get();
    //private Canvas _dragCanvas = H.Property<Canvas>( c => c
        //    .Set( e => {
        //            var canvas = new Canvas();
        //            e._dragDrop.RegisterDragCanvas(canvas);
        //            return canvas;
        //        }
        //    )
        //);

        public Menu Menu { get; } = new Menu {IsMainMenu = true, Background=Brushes.Transparent}; 

    public string Title => _title.Value;
    readonly ObservableAsPropertyHelper<string> _title;

    public ICommand Exit  { get; }

    public ICommand OpenUserCommand { get; } 
}