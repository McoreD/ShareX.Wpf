using System;

namespace ShareX.ScreenCaptureLib
{
    public abstract class Capture
    {
        public DateTime DateTimeCaptured { get; protected set; }
        public string FilePath { get; set; }
    }
}