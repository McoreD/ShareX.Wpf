using HelpersLib;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public sealed class HighlightAnnotation : RectangleAnnotation
    {
        public Color highlightColor { get; set; } = Brushes.Yellow.Color;

        public HighlightAnnotation(ImageEx src)
        {
            CapturedImage = src;

            Stroke = Brushes.Yellow;
            StrokeThickness = 1;
        }

        public override void Render()
        {
            Rect applyRect = AnnotationHelper.CreateIntersectRect(CapturedImage.Size, Area);
            BitmapSource bmp = ImageHelper.CropImage(CapturedImage.Source, applyRect);
            WriteableBitmap wbmp = AnnotationHelper.ChangeColor(bmp, Brushes.Yellow.Color);
            Fill = new ImageBrush(wbmp);
        }
    }
}