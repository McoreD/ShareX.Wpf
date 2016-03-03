using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

        private Point _startPoint;
        private Shape _currentRectangle;
        private Annotate _currentAnnotation;

        private static void ImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CanvasEditor obj = d as CanvasEditor;
            ImageEx img = e.NewValue as ImageEx;
            if (img == null)
            {
                obj.Background = null;
                return;
            }
            obj.Width = img.Source.Width;
            obj.Height = img.Source.Height;
            obj.Background = new ImageBrush(img.Source);
            obj.RedrawAnnotations();
        }

        public void SetAnnotationMode(AnnotationMode mode)
        {
            AnnotationMode = mode;

            switch (mode)
            {
                case AnnotationMode.Highlight:
                    _currentAnnotation = new Highlight();
                    break;
                case AnnotationMode.Obfuscate:
                    _currentAnnotation = new Obfuscate();
                    break;
            }
        }

        private void RedrawAnnotations()
        {
            if (CapturedImage.Annotations == null) { return; }

            RemoveAllAnnotations();

            foreach (var ann in CapturedImage.Annotations)
            {
                if (ann.GetType() == typeof(Highlight))
                {
                    Highlight hl = ann as Highlight;
                    AddShape(hl, hl.TopLeft.X, hl.TopLeft.Y, hl.Width, hl.Height);
                }
            }
        }

        public void RemoveAllAnnotations()
        {
            if (CapturedImage.Annotations == null) { return; }
            Children.RemoveRange(0, Children.Count);
        }

        private Shape AddShape(Annotate ann, double x, double y, double w = 0, double h = 0)
        {
            Shape shape = ann.Render();

            SetLeft(shape, x);
            SetTop(shape, y);
            shape.Width = w;
            shape.Height = h;

            Children.Add(shape);

            return shape;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None) { return; };

            base.OnMouseLeftButtonDown(e);
            _startPoint = e.GetPosition(this);

            _currentRectangle = AddShape(_currentAnnotation, _startPoint.X, _startPoint.Y);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None) { return; };

            base.OnMouseUp(e);

            switch (AnnotationMode)
            {
                case AnnotationMode.Highlight:
                    _currentAnnotation = new Highlight
                    {
                        Width = _currentRectangle.Width,
                        Height = _currentRectangle.Height,
                        TopLeft = _startPoint
                    };
                    break;
                case AnnotationMode.Obfuscate:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            CapturedImage.Annotations.Add(_currentAnnotation);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Released || _currentRectangle == null)
                return;

            bool controlkey = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            var pos = e.GetPosition(this);
            var x = Math.Min(pos.X, _startPoint.X);
            var y = Math.Min(pos.Y, _startPoint.Y);
            var w = Math.Max(pos.X, _startPoint.X) - x;
            var h = Math.Max(pos.Y, _startPoint.Y) - y;

            if (controlkey)
            {
                //make square based on the smallest
                var sml = Math.Min(w, h);
                w = sml;
                h = sml;
            }
            _currentRectangle.Width = w;
            _currentRectangle.Height = h;

            SetLeft(_currentRectangle, x);
            SetTop(_currentRectangle, y);
        }
    }
}