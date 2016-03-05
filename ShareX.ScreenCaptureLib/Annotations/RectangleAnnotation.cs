using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public class RectangleAnnotation : Annotatation
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }
    }
}