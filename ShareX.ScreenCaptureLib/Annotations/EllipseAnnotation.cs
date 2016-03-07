using HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public class EllipseAnnotation : Annotation
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry()
                {
                    Transform = new TranslateTransform(Width / 2, Height / 2),
                    RadiusX = Width / 2,
                    RadiusY = Height / 2
                };
            }
        }

        public EllipseAnnotation()
        {
            brush = Brushes.Red;

            Fill = Brushes.Transparent;
            Stroke = brush;
            StrokeThickness = 1;
        }

        public override void Render(DrawingContext dc)
        {
            dc.DrawImage(GetBitmap(), Bounds);
        }

        public override DrawingVisual GetVisual()
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext dc = visual.RenderOpen())
            {
                dc.DrawImage(GetBitmap(), Bounds);
            }

            return visual;
        }
    }
}