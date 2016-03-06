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

        private AnnotationMode annotationMode = AnnotationMode.None;

        public AnnotationMode AnnotationMode
        {
            get
            {
                return annotationMode;
            }
            set
            {
                annotationMode = value;

                AnnotationHelper.LoadCapturedImage(CapturedImage);
            }
        }

        public ObservableCollection<Annotation> Annotations { get; set; } = new ObservableCollection<Annotation>();

        public bool IsCreatingAnnotation
        {
            get
            {
                return AnnotationMode != AnnotationMode.None && currentAnnotation != null && currentAnnotation.IsCreating;
            }
        }

        private Annotation currentAnnotation;

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

            annotation.IsCreating = true;

            return annotation;
        }

        public void HideAllNodes()
        {
            foreach (Annotation ann in Annotations)
            {
                ann.HideNodes();
            }
        }

        private void UpdateCurrentAnnotation()
        {
            if (currentAnnotation != null)
            {
                SetLeft(currentAnnotation, currentAnnotation.Area.X);
                SetTop(currentAnnotation, currentAnnotation.Area.Y);
                currentAnnotation.Width = currentAnnotation.Area.Width;
                currentAnnotation.Height = currentAnnotation.Area.Height;
            }
        }

        private void FinishCurrentAnnotation()
        {
            if (currentAnnotation != null && currentAnnotation.IsCreating)
            {
                currentAnnotation.IsCreating = false;
                currentAnnotation.FinalRender();
                currentAnnotation.CreateNodes();

                Annotations.Add(currentAnnotation);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left && AnnotationMode != AnnotationMode.None)
            {
                HideAllNodes();
                currentAnnotation = CreateCurrentAnnotation();
                currentAnnotation.PointStart = currentAnnotation.PointFinish = e.GetPosition(this);
                UpdateCurrentAnnotation();

                Children.Add(currentAnnotation);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsCreatingAnnotation)
            {
                currentAnnotation.PointFinish = e.GetPosition(this);
                UpdateCurrentAnnotation();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsCreatingAnnotation)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    currentAnnotation.PointFinish = e.GetPosition(this);
                    UpdateCurrentAnnotation();
                    FinishCurrentAnnotation();
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    Children.Remove(currentAnnotation);
                    currentAnnotation = null;
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (e.LeftButton == MouseButtonState.Pressed && IsCreatingAnnotation)
            {
                FinishCurrentAnnotation();
            }
        }
    }
}