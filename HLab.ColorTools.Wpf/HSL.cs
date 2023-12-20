using System;
using System.Windows.Media;

namespace HLab.ColorTools.Wpf
{
    /// <summary>
    /// Structure to define HSL.
    /// </summary>
    public struct HSL
    {
        /// <summary>
        /// Gets an empty HSL structure;
        /// </summary>
        public static readonly HSL Empty = new HSL();

        double _hue;
        double _saturation;
        double _luminance;
        double _alpha;

        public HSL Saturate(double ratio)
        {
            var hsl = this;
            hsl.Saturation += (1 - hsl.Saturation) * ratio;
            return hsl;
        }

        public HSL Desaturate(double ratio)
        {
            var hsl = this;
            hsl.Saturation *= ratio;
            return hsl;
        }

        public HSL Highlight(double ratio)
        {
            var hsl = this;
            hsl.Luminance += (1 - hsl.Luminance) * ratio;
            return hsl;
        }

        public HSL Darken(double ratio)
        {
            var hsl = this;
            hsl.Luminance *= ratio;
            return hsl;
        }
        public HSL Transparent(double ratio)
        {
            var hsl = this;
            hsl.Alpha *= ratio;
            return hsl;
        }
        public HSL Opaque(double ratio)
        {
            var hsl = this;
            hsl.Alpha += (1 - hsl.Alpha) * ratio;
            return hsl;
        }

        public static bool operator ==(HSL item1, HSL item2)
        {
            return (
                item1.Hue == item2.Hue
                && item1.Saturation == item2.Saturation
                && item1.Luminance == item2.Luminance
                );
        }

        public static bool operator !=(HSL item1, HSL item2)
        {
            return (
                item1.Hue != item2.Hue
                || item1.Saturation != item2.Saturation
                || item1.Luminance != item2.Luminance
                );
        }

        /// <summary>
        /// Gets or sets the hue component.
        /// </summary>
        public double Hue
        {
            get => _hue; set => _hue = (value > 360) ? 360 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets saturation component.
        /// </summary>
        public double Saturation
        {
            get => _saturation; set => _saturation = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Luminance
        {
            get => _luminance; set => _luminance = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }
        /// <summary>
        /// Gets or sets the luminance component.
        /// </summary>
        public double Alpha
        {
            get => _alpha; set => _alpha = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }

        /// <summary>
        /// Creates an instance of a HSL structure.
        /// </summary>
        /// <param name="h">Hue value.</param>
        /// <param name="s">Saturation value.</param>
        /// <param name="l">Lightness value.</param>
        public HSL(double h, double s, double l)
        {
            _hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            _saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            _luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
            _alpha = 1;
        }
        public HSL(double a, double h, double s, double l)
        {
            _hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
            _saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
            _luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
            _alpha = (a > 1) ? 1 : ((a < 0) ? 0 : a);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return (this == (HSL)obj);
        }

        public override int GetHashCode()
        {
            return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
                Luminance.GetHashCode();
        }

        public static implicit operator Color(HSL hsl)
        {
            return hsl.ToColor();
        }

        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        public Color ToColor()
        {
            if (_saturation == 0)
            {
                // achromatic color (gray scale)
                return Color.FromArgb(
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        _alpha * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        _luminance * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        _luminance * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        _luminance * 255.0)))
                    );
            }
            else
            {
                var q = (_luminance < 0.5) ? (_luminance * (1.0 + _saturation)) : (_luminance + _saturation - (_luminance * _saturation));
                var p = (2.0 * _luminance) - q;

                var Hk = _hue / 360.0;
                var T = new double[3];
                T[0] = Hk + (1.0 / 3.0);    // Tr
                T[1] = Hk;                // Tb
                T[2] = Hk - (1.0 / 3.0);    // Tg

                for (var i = 0; i < 3; i++)
                {
                    if (T[i] < 0) T[i] += 1.0;
                    if (T[i] > 1) T[i] -= 1.0;

                    if ((T[i] * 6) < 1)
                    {
                        T[i] = p + ((q - p) * 6.0 * T[i]);
                    }
                    else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i]<0.5
                    {
                        T[i] = q;
                    }
                    else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
                    {
                        T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
                    }
                    else T[i] = p;
                }

                return Color.FromArgb(
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        _alpha * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        T[0] * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        T[1] * 255.0))),
                    Convert.ToByte(double.Parse(string.Format("{0:0.00}",
                        T[2] * 255.0)))
                    );
            }
        }

    }
}
