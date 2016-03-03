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