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

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Shape Render()
        {
            Brush = Brushes.Black;

            return new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                Fill = Brush,
                Opacity = 1
            };
        }
    }
}