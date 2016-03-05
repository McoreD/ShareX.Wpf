using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class ObfuscateAnnotation : RectangleAnnotation
    {
        public ObfuscateAnnotation()
        {
            Brush = Brushes.Black;

            Stroke = Brush;
            StrokeThickness = 1;
        }

        public override void Render()
        {
            Opacity = 1;
            Fill = Brush;
        }
    }
}