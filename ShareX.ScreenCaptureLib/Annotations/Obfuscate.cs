using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public sealed class Obfuscate : Annotate
    {
        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}