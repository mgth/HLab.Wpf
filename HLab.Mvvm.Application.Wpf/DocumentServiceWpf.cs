using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HLab.Core.Annotations;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Application.Documents;
using HLab.Mvvm.Application.Messages;

namespace HLab.Mvvm.Application.Wpf;

public class WpfDocumentService(
        IMvvmService mvvm,
        Func<Type, object> getter,
        IMessagesService messageBus,
    Func<object, ISelectedMessage> getMessage)
    : DocumentService(mvvm, getter)
{
   public IMessagesService MessageBus { get; } = messageBus;
   Func<object, ISelectedMessage> GetMessage { get; } = getMessage;

   static object GetModel(object obj)
   {
      while (true)
      {
         var linked = obj switch
         {
            FrameworkElement fe => fe.DataContext,
            IViewModel vm => vm.Model,
            _ => null
         };

         if (linked is null) return obj;
         obj = linked;
      }
   }

   public override Task OpenDocumentAsync(IView view, IDocumentPresenter presenter)
   {
      if (view is IAnchorableViewClass)
      {
         if (!presenter.Anchorables.Contains(view))
            presenter.Anchorables.Add(view);
      }
      else
      {
         if (!presenter.Documents.Contains(view))
         {
            presenter.Documents.Add(view);

            MessageBus.Publish(GetMessage(view));
         }

         presenter.ActiveDocument = view as FrameworkElement;
      }


      var model = GetModel(view);
      var documents = presenter.Documents.ToList();
      foreach (var document in documents)
      {
         var documentModel = GetModel(document);

         if (!ReferenceEquals(model, documentModel)) continue;

         presenter.ActiveDocument = document;
         return Task.CompletedTask;
      }

      var anchorables = presenter.Anchorables.ToList();
      foreach (var anchorable in anchorables)
      {
         var documentModel = GetModel(anchorable);
         if (ReferenceEquals(model, documentModel))
         {
            return Task.CompletedTask;
         }
      }
      return Task.CompletedTask;
   }


   public override Task CloseDocumentAsync(object content, IDocumentPresenter presenter)
   {
      if (content is IView view)
      {
         if (presenter.Documents.Contains(view))
         {
            presenter.RemoveDocument((FrameworkElement)view);
            return Task.CompletedTask;
         }

         if (presenter.Anchorables.Contains(view))
         {
            presenter.Anchorables.Remove(view);
            return Task.CompletedTask;
         }
      }

      var documents = presenter.Documents.OfType<FrameworkElement>().ToList();
      foreach (var document in documents.Where(IsContent))
      {
            presenter.RemoveDocument(document);
      }

      var anchorables = presenter.Anchorables.OfType<FrameworkElement>().ToList();
      foreach (var anchorable in anchorables.Where(IsContent))
      {
         presenter.Anchorables.Remove(anchorable);
      }

      return Task.CompletedTask;

      bool IsContent(FrameworkElement fe)
      {
         if (ReferenceEquals(fe.DataContext, content)) return true;
         return fe.DataContext is IViewModel mvm && ReferenceEquals(mvm.Model, content);
      }
   }
}