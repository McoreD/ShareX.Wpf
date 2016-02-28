using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public class Highlight
    {
        public Rectangle Rectangle { get; set; }
        public Point TopLeft { get; set; }
        public Brush Color { get; set; }
    }
}