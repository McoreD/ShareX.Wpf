using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelpersLib
{
    public static class Extensions
    {
        public static bool IsValid(this Rect rect)
        {
            return rect.Width > 0 && rect.Height > 0;
        }
    }
}