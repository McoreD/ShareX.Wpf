using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShareX.ScreenCaptureLib
{
    public class Line : Annotate
    {
        protected Geometry cachedGeometry;

        public Line()
        {
            Stroke = Brushes.Blue;
            StrokeThickness = 3;
            StrokeLineJoin = PenLineJoin.Round;
            StrokeStartLineCap = PenLineCap.Flat;
            StrokeEndLineCap = PenLineCap.Flat;
            Fill = Brushes.YellowGreen;

            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Color.FromRgb(10, 10, 10),
                ShadowDepth = 20,
                BlurRadius = 10
            };
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (cachedGeometry == null)
                {
                    CacheDefiningGeometry();
                }
                return cachedGeometry;
            }
        }

        internal virtual void CacheDefiningGeometry()
        {
            Point point1 = new Point(X1, Y1);
            Point point2 = new Point(X2, Y2);

            cachedGeometry = new LineGeometry(point1, point2);
        }
    }
}