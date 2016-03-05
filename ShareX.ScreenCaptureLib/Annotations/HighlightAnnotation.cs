using HelpersLib;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public sealed class HighlightAnnotation : Annotation
    {
        public HighlightAnnotation()
        {
            brush = Brushes.Yellow;

            Stroke = brush;
            StrokeThickness = 1;

            this.MouseUp += HighlightAnnotation_MouseUp;
        }

        private void HighlightAnnotation_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Render();
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                return new RectangleGeometry(new Rect(0, 0, Width, Height));
            }
        }

        public override void Render()
        {
            Rect applyRect = AnnotationHelper.CreateIntersectRect(Area);
            BitmapSource bmp = ImageHelper.CropImage(AnnotationHelper.CapturedImage.Source, applyRect);
            WriteableBitmap wbmp = AnnotationHelper.ChangeColor(bmp, ((SolidColorBrush)brush).Color);
            Fill = new ImageBrush(wbmp);
        }

        public override void Render(DrawingContext dc)
        {
            Render();
            dc.DrawRectangle(Fill, null, Area);
        }
    }
}