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
            get
            {
                return (ImageEx)GetValue(SourceProperty);
            }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        private Annotation currentAnnotation;
        private AdornerLayer adornerLayer;

        public CanvasEditor()
        {
            ClipToBounds = true;
        }

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

        private Annotation CreateCurrentAnnotation()
        {
            Annotation annotation;

            switch (AnnotationMode)
            {
                case AnnotationMode.Highlight:
                    annotation = new HighlightAnnotation();
                    break;
                case AnnotationMode.Obfuscate:
                    annotation = new ObfuscateAnnotation();
                    break;
                case AnnotationMode.Rectangle:
                    annotation = new RectangleAnnotation();
                    break;
                case AnnotationMode.Ellipse:
                    annotation = new EllipseAnnotation();
                    break;
                case AnnotationMode.Arrow:
                    annotation = new ArrowAnnotation();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return annotation;
        }

        private void UpdateCurrentAnnotation()
        {
            SetLeft(currentAnnotation, currentAnnotation.Area.X);
            SetTop(currentAnnotation, currentAnnotation.Area.Y);
            currentAnnotation.Width = currentAnnotation.Area.Width;
            currentAnnotation.Height = currentAnnotation.Area.Height;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            base.OnMouseLeftButtonDown(e);

            currentAnnotation = CreateCurrentAnnotation();
            currentAnnotation.PointStart = currentAnnotation.PointFinish = e.GetPosition(this);
            UpdateCurrentAnnotation();

            Children.Add(currentAnnotation);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None || e.LeftButton == MouseButtonState.Released || currentAnnotation == null)
                return;

            base.OnMouseMove(e);

            currentAnnotation.PointFinish = e.GetPosition(this);
            UpdateCurrentAnnotation();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            base.OnMouseUp(e);

            currentAnnotation.PointFinish = e.GetPosition(this);
            UpdateCurrentAnnotation();
            currentAnnotation.FinalRender();

            CapturedImage.Annotations.Add(currentAnnotation);
            adornerLayer = AdornerLayer.GetAdornerLayer(currentAnnotation);
            adornerLayer.Add(new CircleAdorner(currentAnnotation));
        }
    }
}