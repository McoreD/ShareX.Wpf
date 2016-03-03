using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class Highlight : Annotate
    {
        public Point TopLeft { get; set; }
        public Brush Color { get; set; } = Brushes.Yellow;

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Shape Render()
        {
            return new Rectangle
            {
                Stroke = Brushes.Yellow,
                StrokeThickness = 1,
                Fill = Color,
                Opacity = 0.5
            };
        }
    }
}