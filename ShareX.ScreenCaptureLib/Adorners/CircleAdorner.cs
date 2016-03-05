using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public class CircleAdorner : Adorner
    {
        public CircleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext dc)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush brush = new SolidColorBrush(Colors.Green);
            brush.Opacity = 0.2;
            Pen pen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double r = 5.0;

            // Draw a circle at each corner.
            dc.DrawEllipse(brush, pen, adornedElementRect.TopLeft, r, r);
            dc.DrawEllipse(brush, pen, adornedElementRect.TopRight, r, r);
            dc.DrawEllipse(brush, pen, adornedElementRect.BottomLeft, r, r);
            dc.DrawEllipse(brush, pen, adornedElementRect.BottomRight, r, r);
        }
    }
}