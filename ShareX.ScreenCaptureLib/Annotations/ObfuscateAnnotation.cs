using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class ObfuscateAnnotation : Annotation
    {
        public ObfuscateAnnotation()
        {
            Brush = Brushes.Black;

            Stroke = Brush;
            StrokeThickness = 1;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public override void Render()
        {
            Opacity = 1;
            Fill = Brush;
        }

        public override void Render(DrawingContext dc)
        {
            Render();
            dc.DrawRectangle(Fill, null, Area);
        }
    }
}