using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public abstract class Annotate : Shape
    {
        public virtual Shape Render()
        {
            throw new NotImplementedException();
        }
    }
}