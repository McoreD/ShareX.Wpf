using HelpersLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        private AnnotationMode annotationMode = AnnotationMode.Cursor;

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
                return AnnotationMode != AnnotationMode.Cursor && currentAnnotation != null && currentAnnotation.IsCreating;
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

        public void UnselectAll()
        {
            foreach (Annotation ann in Annotations)
            {
                ann.Selected = false;
            }
        }

        public void DeleteSelected()
        {
            foreach (Annotation ann in Annotations.Where(x => x.Selected).ToArray())
            {
                Annotations.Remove(ann);
                Children.Remove(ann);
            }
        }

        private void FinishCurrentAnnotation()
        {
            if (currentAnnotation != null && currentAnnotation.IsCreating)
            {
                currentAnnotation.IsCreating = false;
                currentAnnotation.FinalRender();
                currentAnnotation.Selected = true;

                Annotations.Add(currentAnnotation);
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left && AnnotationMode != AnnotationMode.Cursor)
            {
                UnselectAll();
                currentAnnotation = CreateCurrentAnnotation();
                currentAnnotation.PointStart = currentAnnotation.PointFinish = e.GetPosition(this);
                currentAnnotation.UpdateDimension();

                Children.Add(currentAnnotation);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsCreatingAnnotation)
            {
                currentAnnotation.PointFinish = e.GetPosition(this);
                currentAnnotation.UpdateDimension();
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
                    currentAnnotation.UpdateDimension();
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Delete)
            {
                DeleteSelected();
            }
        }
    }
}