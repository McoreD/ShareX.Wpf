using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public class Arrow : Line
    {
        internal override void CacheDefiningGeometry()
        {
            Point startPoint = new Point(X1, Y1);
            Point endPoint = new Point(X2, Y2);

            var length = Math.Sqrt(Math.Pow(endPoint.Y - startPoint.Y, 2) + Math.Pow(endPoint.X - startPoint.X, 2));
            var f = length / 15d;
            var f2 = length / 30d;

            var arrowBase = new Point(endPoint.X - (endPoint.X - startPoint.X) / f2, endPoint.Y - (endPoint.Y - startPoint.Y) / f2);
            var arrowTip1 = new Point(arrowBase.X + (endPoint.Y - startPoint.Y) / f, arrowBase.Y - (endPoint.X - startPoint.X) / f);
            var arrowTip2 = new Point(arrowBase.X - (endPoint.Y - startPoint.Y) / f, arrowBase.Y + (endPoint.X - startPoint.X) / f);
            var arrowTipExt1 = new Point(arrowBase.X + (endPoint.Y - startPoint.Y) / f2, arrowBase.Y - (endPoint.X - startPoint.X) / f2);
            var arrowTipExt2 = new Point(arrowBase.X - (endPoint.Y - startPoint.Y) / f2, arrowBase.Y + (endPoint.X - startPoint.X) / f2);

            var arrow = new PathFigure(startPoint, new[] { new PolyLineSegment(new[] { arrowTip1, arrowTipExt1, endPoint, arrowTipExt2, arrowTip2 }, true), }, true)
            {
                IsFilled = true
            };
            cachedGeometry = new PathGeometry(new[] { arrow }, FillRule.Nonzero, null);
        }
    }
}