using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLib
{
    public static class HelpersOptions
    {
        public static ProxyInfo CurrentProxy = new ProxyInfo();
        public static bool DefaultCopyImageFillBackground = true;
        public static bool UseAlternativeCopyImage = true;
        public static bool UseAlternativeGetImage = true;
        public static string BrowserPath = null;
    }
}