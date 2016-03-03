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
    public sealed class Obfuscate : Annotate
    {
        public Point TopLeft { get; set; }
        public Brush Color { get; set; } = Brushes.Black;

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Shape Render()
        {
            return new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Color,
                Opacity = 1
            };
        }
    }
}