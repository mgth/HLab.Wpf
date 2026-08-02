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
      // Chercher un document existant pour le même modèle AVANT d'ajouter : les
      // vues ne sont pas cachées (IView NotCacheable), chaque GetView en crée une
      // nouvelle — ajouter d'abord laissait un doublon dans le présenteur, et
      // AvalonDock crashait au réattachement (« déjà l'enfant logique »).
      var model = GetModel(view);

      if (view is IAnchorableViewClass)
      {
         if (presenter.Anchorables.Contains(view)) return Task.CompletedTask;

         foreach (var anchorable in presenter.Anchorables.ToList())
         {
            if (ReferenceEquals(model, GetModel(anchorable)))
               return Task.CompletedTask;
         }

         presenter.Anchorables.Add(view);
         return Task.CompletedTask;
      }

      foreach (var document in presenter.Documents.ToList())
      {
         if (!ReferenceEquals(model, GetModel(document))) continue;

         presenter.ActiveDocument = document;
         return Task.CompletedTask;
      }

      presenter.Documents.Add(view);
      MessageBus.Publish(GetMessage(view));
      presenter.ActiveDocument = view as FrameworkElement;

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