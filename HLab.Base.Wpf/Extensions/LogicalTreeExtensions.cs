using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HLab.Base.Wpf.Extensions;

public static class LogicalTreeExtensions
{
   public static IEnumerable<T> FindVisualChildren<T>(this ItemsControl itemsControl) where T : class
   {
      for (var i = 0; i < itemsControl.Items.Count; i++)
      {
         if (itemsControl.ItemContainerGenerator.ContainerFromIndex(i) is not ContentPresenter { Content: object obj } presenter) continue;
         foreach (var child in presenter.FindVisualChildren<T>())
         {
            yield return child;
         }
      }
   }


   public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject? dependencyObject) where T : class//where T : DependencyObject
   {
      if (dependencyObject is T c)
      {
         yield return c;
      }

      switch (dependencyObject)
      {
         case null:
            break;
         case ItemsControl itemsControl:
            foreach (var child in itemsControl.FindVisualChildren<T>())
            {
               yield return child;
            }
            break;
         default:
            foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
            {
                  foreach (var c1 in child.FindVisualChildren<T>())
                  {
                     yield return c1;
                  }
            }
            break;
      }
   }

   public static DependencyObject FindVisualTreeRoot(this DependencyObject initial)
   {
      var dependencyObject = initial;
      var result = initial;
      while (dependencyObject != null)
      {
         result = dependencyObject;
         dependencyObject = dependencyObject is not Visual && dependencyObject is not Visual3D ? LogicalTreeHelper.GetParent(dependencyObject) : VisualTreeHelper.GetParent(dependencyObject);
      }

      return result;
   }

   public static T? FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class
   {
      var dependencyObject2 = dependencyObject;
      do
      {
         dependencyObject2 = VisualTreeHelper.GetParent(dependencyObject2);
      }
      while (dependencyObject2 != null && !(dependencyObject2 is T));
      return dependencyObject2 as T;
   }

   public static T? FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class
   {
      var dependencyObject2 = dependencyObject;
      do
      {
         var reference = dependencyObject2;
         dependencyObject2 = LogicalTreeHelper.GetParent(dependencyObject2) ?? VisualTreeHelper.GetParent(reference);
      }
      while (dependencyObject2 != null && !(dependencyObject2 is T));
      return dependencyObject2 as T;
   }

}