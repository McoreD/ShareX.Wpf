using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotation : Annotation
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public RectangleAnnotation()
        {
            Brush = Brushes.Red;

            Stroke = Brush;
            StrokeThickness = 1;
        }

        public override void Render()
        {
            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Color.FromRgb(10, 10, 10),
                ShadowDepth = 7,
                BlurRadius = 5
            };
        }

        public override void Render(DrawingContext dc)
        {
            dc.DrawRectangle(Fill, null, Area);
        }
    }
}