using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public class LineAnnotation : Annotation
    {
        protected Geometry cachedGeometry;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new LineGeometry(PointStart, PointFromScreen(CaptureHelper.GetCursorPosition()));
            }
        }

        public LineAnnotation()
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
                ShadowDepth = 0,
                BlurRadius = 10
            };
        }

        internal virtual void CacheDefiningGeometry()
        {
            cachedGeometry = new LineGeometry(PointStart, PointFinish);
        }
    }
}