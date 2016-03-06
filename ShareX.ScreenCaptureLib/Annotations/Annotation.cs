using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    public abstract class Annotation : Shape, IAnnotation
    {
        protected Brush brush;
        protected Adorner adorner;

        public bool IsCreating { get; set; }

        private bool selected;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;

                if (selected)
                {
                    ShowNodes();
                }
                else
                {
                    HideNodes();
                }
            }
        }

        public Point PointStart
        {
            get { return new Point(X1, Y1); }
            set { X1 = value.X; Y1 = value.Y; }
        }

        public Point PointFinish
        {
            get { return new Point(X2, Y2); }
            set { X2 = value.X; Y2 = value.Y; }
        }

        public Rect Area
        {
            get
            {
                return CaptureHelper.CreateRectangle(PointStart, PointFinish);
            }
        }

        protected void CreateNodes()
        {
            adorner = new CircleAdorner(this);
            AdornerLayer.GetAdornerLayer(this).Add(adorner);
        }

        protected void ShowNodes()
        {
            if (adorner == null)
            {
                CreateNodes();
            }

            adorner.Visibility = Visibility.Visible;
        }

        protected void HideNodes()
        {
            if (adorner == null)
            {
                CreateNodes();
            }

            adorner.Visibility = Visibility.Hidden;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                Selected = !Selected;
            }
        }

        internal static bool IsDoubleFinite(object o)
        {
            double d = (double)o;
            return (!double.IsInfinity(d) && !double.IsNaN(d));
        }

        public virtual RenderTargetBitmap FinalRender()
        {
            var rtb = new RenderTargetBitmap((int)Width, (int)Height, AnnotationHelper.CapturedImage.Source.DpiX, AnnotationHelper.CapturedImage.Source.DpiY, PixelFormats.Pbgra32);
            rtb.Render(this);
            return rtb;
        }

        public abstract void FinalRender(DrawingContext dc);

        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Annotation),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            new ValidateValueCallback(IsDoubleFinite));

        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Annotation),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            new ValidateValueCallback(IsDoubleFinite));

        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Annotation),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            new ValidateValueCallback(IsDoubleFinite));

        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Annotation),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender),
            new ValidateValueCallback(IsDoubleFinite));

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
    }
}