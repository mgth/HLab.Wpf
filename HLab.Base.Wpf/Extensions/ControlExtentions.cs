using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HLab.Base.Wpf.Extensions
{
    public static class ControlExtensions
    {
        //public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) 
        //{
        //    if (depObj == null) yield break;

        //    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        //    {
        //        var child = VisualTreeHelper.GetChild(depObj, i);
        //        if (child is T childT)
        //        {
        //            yield return childT;
        //        }

        //        foreach (var childOfChild in child.FindVisualChildren<T>())
        //        {
        //            yield return childOfChild;
        //        }
        //    }
        //}

        public static void SwitchVisibility(this UIElement element)
        {
            element.Visibility = element.Visibility switch
            {
                Visibility.Hidden => Visibility.Hidden,
                Visibility.Collapsed => Visibility.Visible,
                Visibility.Visible => Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject? depObj) where T : class//where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T val)
                {
                    yield return val;
                }

                foreach (var item in child.FindVisualChildren<T>())
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(this DependencyObject? depObj) where T : class//where T : DependencyObject
        {
            if (depObj == null)
            {
                yield break;
            }

            foreach (var child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
            {
                if (child is T val)
                {
                    yield return val;
                }

                foreach (var item in child.FindLogicalChildren<T>())
                {
                    yield return item;
                }
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
}
