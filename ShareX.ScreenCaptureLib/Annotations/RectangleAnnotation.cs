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
        public int ShadowSize { get; set; } = 15;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public RectangleAnnotation()
        {
            brush = FindResource("PrimaryHueDarkBrush") as Brush;

            Fill = Brushes.Transparent;
            Stroke = brush;
            StrokeThickness = 1;

            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Colors.Black,
                ShadowDepth = 0,
                BlurRadius = ShadowSize
            };
        }
    }
}