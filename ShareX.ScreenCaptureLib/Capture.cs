using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib
{
    public abstract class Capture
    {
        public DateTime DateTimeCaptured { get; protected set; }
    }
}