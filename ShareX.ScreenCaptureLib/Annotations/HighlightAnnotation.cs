using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class HighlightAnnotation : RectangleAnnotation
    {
        public Color highlightColor { get; set; } = Brushes.Yellow.Color;

        public override Shape Render()
        {
            Brush = Brushes.Yellow;

            return new Rectangle
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 1,
            };
        }
    }
}