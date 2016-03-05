using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class Highlight : Annotate
    {
        public Color highlightColor { get; set; } = Brushes.Yellow.Color;

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Shape Render()
        {
            Brush = Brushes.Yellow;

            return new Rectangle
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 1,
                // Fill = Brush,
                // Opacity = 0.5
            };
        }
    }
}