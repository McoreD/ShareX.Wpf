using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    internal interface IAnnotation
    {
        Shape Render();
    }
}