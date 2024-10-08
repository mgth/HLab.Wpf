using System;
using HashCode = HLab.Base.HashCode;

namespace HLab.Mvvm.Wpf.Factory
{
    internal class LinkKey(Type viewClass, Type viewMode)
    {
        public Type ViewClass { get; } = viewClass;
        public Type ViewMode { get; } = viewMode;

        public override bool Equals(object other)
        {
            if (other is LinkKey key)
            {
                return Equals(key);
            }
            return false;
        }

        protected bool Equals(LinkKey other)
        {
            return ViewClass == other.ViewClass && ViewMode == other.ViewMode;
        }

        public override int GetHashCode()
        {
                return HashCode.Start.Add(ViewClass).Add(ViewMode).Value;
        }

        public bool IsAssignableFrom(LinkKey other) => ViewClass.IsAssignableFrom(other.ViewClass) && ViewMode.IsAssignableFrom(other.ViewMode);
    }
}