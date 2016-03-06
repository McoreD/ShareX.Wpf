using HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ScreenCaptureLib
{
    public class WindowInfo
    {
        public IntPtr Handle { get; private set; }

        public string Text
        {
            get
            {
                return NativeMethods.GetWindowText(Handle);
            }
        }

        public string ClassName
        {
            get
            {
                return NativeMethods.GetClassName(Handle);
            }
        }

        public Process Process
        {
            get
            {
                return NativeMethods.GetProcessByWindowHandle(Handle);
            }
        }

        public string ProcessName
        {
            get
            {
                Process process = Process;

                if (process != null)
                {
                    return process.ProcessName;
                }

                return null;
            }
        }

        public Rect Rectangle
        {
            get
            {
                return CaptureHelper.GetWindowRectangle(Handle);
            }
        }

        public Rect Rectangle0Based
        {
            get
            {
                return CaptureHelper.ScreenToClient(Rectangle);
            }
        }

        public Rect ClientRectangle
        {
            get
            {
                return NativeMethods.GetClientRect(Handle);
            }
        }

        public WindowStyles Styles
        {
            get
            {
                return (WindowStyles)NativeMethods.GetWindowLong(Handle, NativeMethods.GWL_STYLE);
            }
        }

        public bool IsMaximized
        {
            get
            {
                return NativeMethods.IsZoomed(Handle);
            }
        }

        public bool IsMinimized
        {
            get
            {
                return NativeMethods.IsIconic(Handle);
            }
        }

        public bool IsVisible
        {
            get
            {
                return NativeMethods.IsWindowVisible(Handle);
            }
        }

        public WindowInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public void Activate()
        {
            NativeMethods.ActivateWindow(Handle);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}