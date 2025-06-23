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
   public class DocumentPresenter(IMessagesService message, Func<object, ISelectedMessage> getSelectedMessage)
      : ViewModel, IDocumentPresenter
   {
      public ObservableCollection<object> Documents { get; } = [];
      public ObservableCollection<object> Anchorables { get; } = [];

      readonly List<object> _documentHistory = [];

      public object? ActiveDocument
      {
         get;
         set
         {
            if (value is not null)
            {
               _documentHistory.Remove(value);
               _documentHistory.Insert(0, value);
            }
            if (!this.SetAndRaise(ref field, value)) return;

            message.Publish(getSelectedMessage(value));
         }
      }
      
      public object? Theme {get; set => this.SetAndRaise(ref field,value); }

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
