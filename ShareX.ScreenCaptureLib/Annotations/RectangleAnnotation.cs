using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotation : Annotation
    {
        public int ShadowSize = 10;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Math.Max(0, Width - ShadowSize), Math.Max(0, Height - ShadowSize)));
            }
        }

        public RectangleAnnotation()
        {
            brush = Brushes.Red;

            Stroke = brush;
            StrokeThickness = 1;
        }

        public override void Render()
        {
            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Colors.Black,
                ShadowDepth = 0,
                BlurRadius = ShadowSize,
                Direction = 45
            };
        }

        public override void Render(DrawingContext dc)
        {
            Render();
            dc.DrawRectangle(Fill, null, Area);
        }
    }
}