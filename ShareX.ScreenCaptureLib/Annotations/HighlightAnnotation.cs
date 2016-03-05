using HelpersLib;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShareX.ScreenCaptureLib
{
    public sealed class HighlightAnnotation : RectangleAnnotation
    {
        public HighlightAnnotation()
        {
            Brush = Brushes.Yellow;

            Stroke = Brush;
            StrokeThickness = 1;
        }

        public override void Render()
        {
            Rect applyRect = AnnotationHelper.CreateIntersectRect(Area);
            BitmapSource bmp = ImageHelper.CropImage(AnnotationHelper.CapturedImage.Source, applyRect);
            WriteableBitmap wbmp = AnnotationHelper.ChangeColor(bmp, ((SolidColorBrush)Brush).Color);
            Fill = new ImageBrush(wbmp);
        }
    }
}