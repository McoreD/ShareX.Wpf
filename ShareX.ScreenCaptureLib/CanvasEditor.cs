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
                case AnnotationMode.Arrow:
                    currentAnnotation = new ArrowAnnotation();
                    break;
                default:
                    throw new NotImplementedException();
            }

            currentAnnotation.PointStart = pStart;

            SetLeft(currentAnnotation, pStart.X);
            SetTop(currentAnnotation, pStart.Y);

            Children.Add(currentAnnotation);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None) { return; };

            base.OnMouseUp(e);

            currentAnnotation.PointFinish = e.GetPosition(this);

            currentAnnotation.Render();

            CapturedImage.Annotations.Add(currentAnnotation);
            adornerLayer = AdornerLayer.GetAdornerLayer(currentAnnotation);
            adornerLayer.Add(new CircleAdorner(currentAnnotation));

            if (CapturedImage.Annotations.Count == 1)
            {
                adornerLayer.MouseUp += AdornerLayer_MouseUp;
                adornerLayer.MouseDown += AdornerLayer_MouseDown;
                adornerLayer.Cursor = Cursors.SizeAll;
            }
        }

        private void AdornerLayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentAnnotation.Fill = null;
        }

        private void AdornerLayer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            currentAnnotation.PointFinish = e.GetPosition(this);
            currentAnnotation.Render();
        }

        private void UpdateDimensions(Point pos)
        {
            var x = Math.Min(pos.X, pStart.X);
            var y = Math.Min(pos.Y, pStart.Y);
            var w = Math.Max(pos.X, pStart.X) - x;
            var h = Math.Max(pos.Y, pStart.Y) - y;

            currentAnnotation.Width = w;
            currentAnnotation.Height = h;

            SetLeft(currentAnnotation, x);
            SetTop(currentAnnotation, y);
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