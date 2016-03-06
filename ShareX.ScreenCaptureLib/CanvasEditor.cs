using HelpersLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public class CanvasEditor : Canvas
    {
        public delegate void ImageLoadedEventHandler();
        public event ImageLoadedEventHandler ImageLoaded;

        public AnnotationMode AnnotationMode { get; private set; } = AnnotationMode.None;

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageEx), typeof(CanvasEditor), new FrameworkPropertyMetadata(ImagePropertyChangedCallback));

        [Category("Editor")]
        public ImageEx CapturedImage
        {
            get { return (ImageEx)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private Point pStart;
        private Annotation currentAnnotation;
        private AdornerLayer adornerLayer;

        protected virtual void OnImageLoaded()
        {
            if (ImageLoaded != null) ImageLoaded();
        }

        private static void ImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CanvasEditor canvas = d as CanvasEditor;
            ImageEx img = e.NewValue as ImageEx;

            if (img == null)
            {
                canvas.Background = null;
                return;
            }

            canvas.Width = img.Source.Width;
            canvas.Height = img.Source.Height;
            canvas.Background = new ImageBrush(img.Source);
            canvas.ImageLoaded();

            canvas.Children.Clear();
        }

        public void Init(AnnotationMode mode)
        {
            AnnotationMode = mode;
            AnnotationHelper.LoadCapturedImage(CapturedImage);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None) { return; };

            base.OnMouseLeftButtonDown(e);
            pStart = e.GetPosition(this);

            switch (AnnotationMode)
            {
                case AnnotationMode.Highlight:
                    currentAnnotation = new HighlightAnnotation();
                    break;
                case AnnotationMode.Obfuscate:
                    currentAnnotation = new ObfuscateAnnotation();
                    break;
                case AnnotationMode.Rectangle:
                    currentAnnotation = new RectangleAnnotation();
                    break;
                case AnnotationMode.Ellipse:
                    currentAnnotation = new EllipseAnnotation();
                    break;
                case AnnotationMode.Arrow:
                    currentAnnotation = new ArrowAnnotation();
                    break;
                default:
                    throw new NotImplementedException();
            }

            currentAnnotation.PointStart = e.GetPosition(this);
            currentAnnotation.CursorPosStart = CaptureHelper.GetCursorPosition();

            Console.WriteLine($"PointStart {CaptureHelper.GetCursorPosition()}");
            Console.WriteLine($"PointFromScreen(CaptureHelper.GetCursorPosition() {PointFromScreen(CaptureHelper.GetCursorPosition())}");
            Console.WriteLine($"GetPosition {e.GetPosition(this)}");

            SetLeft(currentAnnotation, pStart.X);
            SetTop(currentAnnotation, pStart.Y);

            Children.Add(currentAnnotation);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None) { return; };

            base.OnMouseUp(e);

            currentAnnotation.PointFinish = e.GetPosition(this);
            Console.WriteLine($"PointFinish {CaptureHelper.GetCursorPosition()}");
            Console.WriteLine($"PointFromScreen(CaptureHelper.GetCursorPosition() {PointFromScreen(CaptureHelper.GetCursorPosition())}");
            Console.WriteLine($"GetPosition {e.GetPosition(this)}");

            currentAnnotation.FinalRender();

            CapturedImage.Annotations.Add(currentAnnotation);
            adornerLayer = AdornerLayer.GetAdornerLayer(currentAnnotation);
            adornerLayer.Add(new CircleAdorner(currentAnnotation));

            UpdateDimensions(e.GetPosition(this));
        }

        private void UpdateDimensions(Point pos)
        {
            var x = Math.Min(pos.X, pStart.X);
            var y = Math.Min(pos.Y, pStart.Y);
            var w = Math.Max(pos.X, pStart.X) - x;
            var h = Math.Max(pos.Y, pStart.Y) - y;

            currentAnnotation.Width = w;
            currentAnnotation.Height = h;

            //  SetLeft(currentAnnotation, x); // needs to be relative to canvas
            //  SetTop(currentAnnotation, y);  // needs to be relative to canvas
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            if (e.LeftButton == MouseButtonState.Released || currentAnnotation == null)
                return;

            base.OnMouseMove(e);

            UpdateDimensions(e.GetPosition(this));
        }
    }
}