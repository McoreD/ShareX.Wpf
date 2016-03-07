using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelpersLib
{
    public static class WindowExtensions
    {
        public static void Bounds(this Window window, Rect ScreenRectangle)
        {
            window.Left = ScreenRectangle.Left;
            window.Top = ScreenRectangle.Top;
            window.Width = ScreenRectangle.Width;
            window.Height = ScreenRectangle.Height;
        }
    }
}