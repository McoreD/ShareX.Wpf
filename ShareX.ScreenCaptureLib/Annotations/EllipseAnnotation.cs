using HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShareX.ScreenCaptureLib
{
    public class EllipseAnnotation : Annotation
    {
        public int ShadowSize { get; set; } = 15;

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new EllipseGeometry()
                {
                    Transform = new TranslateTransform(Width / 2, Height / 2),
                    RadiusX = Width / 2,
                    RadiusY = Height / 2
                };
            }
        }

        public EllipseAnnotation()
        {
            brush = Brushes.Red;

            Fill = Brushes.Transparent;
            Stroke = brush;
            StrokeThickness = 1;

            Effect = new DropShadowEffect
            {
                RenderingBias = RenderingBias.Quality,
                Opacity = 0.8d,
                Color = Colors.Black,
                ShadowDepth = 0,
                BlurRadius = ShadowSize
            };
        }

        public override DrawingVisual Render()
        {
            DrawingVisual visual = new DrawingVisual();
            visual.Effect = Effect;

            return visual;
        }
    }
}