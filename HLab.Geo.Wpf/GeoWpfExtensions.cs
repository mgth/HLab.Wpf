
using System.Runtime.CompilerServices;

namespace HLab.Geo.Wpf;

public static class GeoWpfExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Point ToGeoPoint(this System.Windows.Point point) => Unsafe.As<System.Windows.Point, Point>(ref point);

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static System.Windows.Point ToWpfPoint(this Point point) => Unsafe.As<Point, System.Windows.Point>(ref point);
}