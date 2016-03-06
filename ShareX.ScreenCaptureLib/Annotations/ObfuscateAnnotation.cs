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
            brush = Brushes.Black;

            Stroke = brush;
            StrokeThickness = 1;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public override RenderTargetBitmap Render()
        {
            Opacity = 1;
            Fill = brush;

            return base.Render();
        }

        public override void Render(DrawingContext dc)
        {
            // dc.DrawImage(Render(), new Rect(0, 0, Width, Height));
            Render();
            dc.DrawRectangle(Fill, null, Area);
        }
    }
}