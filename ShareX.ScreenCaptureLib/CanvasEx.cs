using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public delegate void AddHighlightEventHandler(object sender, AddHighlightEventArgs args);

    public class AddHighlightEventArgs : EventArgs
    {
        public Highlight TheHighlight { get; private set; }

        public AddHighlightEventArgs(Highlight theHighlight)
        {
            TheHighlight = theHighlight;
        }
    }

    public class CanvasEx : Canvas
    {
        public AnnotationMode AnnotationMode { get; set; } = AnnotationMode.None;
        public event AddHighlightEventHandler HighlightAdded = delegate { };

        public static readonly DependencyProperty ImageProperty;
        public static readonly DependencyProperty HighlightColorProperty;
        public static readonly DependencyProperty HighlightsProperty;

        [Category("Source")]
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        [Category("Annotations")]
        public string HighlightColor
        {
            get { return (string)GetValue(HighlightColorProperty); }
            set { SetValue(HighlightColorProperty, value); }
        }

        [Category("Annotations")]
        public ObservableCollection<Highlight> Highlights
        {
            get { return (ObservableCollection<Highlight>)GetValue(HighlightsProperty); }
            set { SetValue(HighlightsProperty, value); }
        }

        [Category("Annotations")]
        private Brush HighlightColorBrush
        {
            get { return (SolidColorBrush)new BrushConverter().ConvertFromString(HighlightColor); }
        }

        private Point _startPoint;
        private Rectangle _currentRectangle;
        private Highlight _highlightBeingAdded;

        static CanvasEx()
        {
            ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(CanvasEx), new FrameworkPropertyMetadata(ImagePropertyChangedCallback));
            HighlightColorProperty = DependencyProperty.Register("HighlightColor", typeof(string), typeof(CanvasEx));
            HighlightsProperty = DependencyProperty.Register("Highlights", typeof(ObservableCollection<Highlight>), typeof(CanvasEx));
        }

        public CanvasEx()
        {
            Highlights = new ObservableCollection<Highlight>();
        }

        private static void ImagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CanvasEx obj = d as CanvasEx;
            ImageSource img = e.NewValue as ImageSource;
            if (img == null)
            {
                obj.Background = null;
                return;
            }
            obj.Width = img.Width;
            obj.Height = img.Height;
            obj.Background = new ImageBrush(img);
            obj.RedrawHighlights();
        }

        private void RedrawHighlights()
        {
            RemoveAllHighlights();
            if (Highlights == null)
            {
                return;
            }
            foreach (var hl in Highlights)
            {
                AddNewRectangle(hl.Color, hl.TopLeft.X, hl.TopLeft.Y, hl.Rectangle.Width, hl.Rectangle.Height);
            }
        }

        public void RemoveAllHighlights()
        {
            if (Highlights == null) { return; }
            Children.RemoveRange(0, Children.Count);
        }

        private Rectangle AddNewRectangle(Brush color, double x, double y, double w = 0, double h = 0)
        {
            var r = new Rectangle
            {
                Stroke = Brushes.LightBlue,
                StrokeThickness = 1,
                Fill = color,
                Opacity = 0.5
            };

            SetLeft(r, x);
            SetTop(r, y);
            r.Width = w;
            r.Height = h;
            Children.Add(r);
            return r;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            base.OnMouseLeftButtonDown(e);
            _startPoint = e.GetPosition(this);

            _currentRectangle = AddNewRectangle(HighlightColorBrush, _startPoint.X, _startPoint.Y);

            _highlightBeingAdded = new Highlight
            {
                Rectangle = _currentRectangle,
                TopLeft = _startPoint,
                Color = HighlightColorBrush
            };
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (AnnotationMode == AnnotationMode.None)
                return;

            base.OnMouseUp(e);
            HighlightAdded(this, new AddHighlightEventArgs(_highlightBeingAdded));
            _highlightBeingAdded = null;
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