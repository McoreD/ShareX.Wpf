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

        public static Version Normalize(this Version version)
        {
            return new Version(Math.Max(version.Major, 0), Math.Max(version.Minor, 0), Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
        }

        public static long ToUnix(this DateTime dateTime)
        {
            return Helper.DateTimeToUnix(dateTime);
        }
    }
}