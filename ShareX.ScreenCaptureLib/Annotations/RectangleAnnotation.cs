using HelpersLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotation : Annotation
    {
        public int ShadowSize = 15;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public RectangleAnnotation()
        {
            brush = Brushes.Red;

            Stroke = brush;
            StrokeThickness = 1;
        }

        public override RenderTargetBitmap FinalRender()
        {
            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Colors.Black,
                ShadowDepth = 0,
                BlurRadius = ShadowSize
            };

            return base.FinalRender();
        }

        public override void FinalRender(DrawingContext dc)
        {
            FinalRender();
            dc.DrawRectangle(null, new Pen(brush, StrokeThickness), Area);
        }
    }
}