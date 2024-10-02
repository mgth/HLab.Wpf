using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HLab.Base.ReactiveUI;
using HLab.Core.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Messages;
using HLab.Mvvm.ReactiveUI;


namespace HLab.Mvvm.Application.Wpf
{
    
    public class DocumentPresenter : ViewModel, IDocumentPresenter
    {
        readonly IMessagesService _message;
        readonly Func<object, ISelectedMessage> _getSelectedMessage;

        public DocumentPresenter
        (
            IMessagesService message,             
            Func<object, ISelectedMessage> getSelectedMessage 
        )
        {
            _message = message;
            _getSelectedMessage = getSelectedMessage;

            
        }

        public ObservableCollection<object> Documents { get; } = new();
        public ObservableCollection<object> Anchorables { get; } = new();

        readonly List<object> _documentHistory = new();

        public object ActiveDocument
        {
            get => _activeDocument;
            set
            {
                _documentHistory.Remove(value);
                _documentHistory.Insert(0, value);

                if (!this.SetAndRaise(ref _activeDocument,value)) return;

                _message.Publish(_getSelectedMessage(value));
            }
        }
        object _activeDocument ;

        public bool RemoveDocument(object document)
        {
            if (!Documents.Contains(document)) return false;
            if (_documentHistory.Count <= 0 || !ReferenceEquals(_documentHistory[0], document)) return false;

            _documentHistory.Remove(document);
            if (_documentHistory.Count > 0)
            {
                ActiveDocument = _documentHistory[0];
            }
            Documents.Remove(document);

            return true;
        }

    }
}
