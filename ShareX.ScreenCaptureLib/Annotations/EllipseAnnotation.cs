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
                return new EllipseGeometry(PointFromScreen(CaptureHelper.GetCursorPosition()), Width, Height);
            }
        }

        public EllipseAnnotation()
        {
            brush = Brushes.Blue;
            Stroke = brush;
            StrokeThickness = 1;
        }

        public override void FinalRender(DrawingContext dc)
        {
            throw new NotImplementedException();
        }
    }
}