using HelpersLib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShareX.ScreenCaptureLib
{
    /// <summary>
    /// Interaction logic for RectangleLight.xaml
    /// </summary>
    public partial class RectangleLight : Window
    {
        public static Rect LastSelectionRectangle0Based { get; private set; }
        public Rect ScreenRectangle { get; private set; }

        public Rect ScreenRectangle0Based
        {
            get
            {
                return new Rect(0, 0, ScreenRectangle.Width, ScreenRectangle.Height);
            }
        }

        public Rect SelectionRectangle { get; private set; }

        public Rect SelectionRectangle0Based
        {
            get
            {
                return new Rect(SelectionRectangle.X - ScreenRectangle.X, SelectionRectangle.Y - ScreenRectangle.Y, SelectionRectangle.Width, SelectionRectangle.Height);
            }
        }

        private BitmapSource backgroundImage;
        private Point currentPosition, positionOnClick;
        private bool isMouseDown;
        private Rectangle CropArea = new Rectangle();

        public RectangleLight()
        {
            backgroundImage = ScreenshotHelper.CaptureFullscreen().Source;
            Background = new ImageBrush(backgroundImage);

            ScreenRectangle = CaptureHelper.GetScreenBounds();

            InitializeComponent();

            CropArea.Stroke = Brushes.Red;
            CropArea.Fill = new SolidColorBrush() { Color = Color.FromArgb(50, 132, 112, 255) };
        }

        protected override void OnInitialized(EventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            this.Bounds(ScreenRectangle);

            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Topmost = true;

            base.OnInitialized(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            ShowActivated = true;
            base.OnContentRendered(e);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                positionOnClick = CaptureHelper.GetCursorPosition();
                isMouseDown = true;
                canvas.Children.Add(CropArea);
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                // canvas.Children.Remove(CropArea);
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                currentPosition = CaptureHelper.GetCursorPosition();
                SelectionRectangle = CaptureHelper.CreateRectangle(positionOnClick.X, positionOnClick.Y, currentPosition.X, currentPosition.Y);

                CropArea.SetValue(Canvas.LeftProperty, SelectionRectangle0Based.Left);
                CropArea.SetValue(Canvas.TopProperty, SelectionRectangle0Based.Top);
                CropArea.Width = SelectionRectangle0Based.Width;
                CropArea.Height = SelectionRectangle0Based.Height;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                if (isMouseDown)
                {
                    if (SelectionRectangle0Based.Width > 0 && SelectionRectangle0Based.Height > 0)
                    {
                        LastSelectionRectangle0Based = SelectionRectangle0Based;
                        DialogResult = true;
                    }

                    Close();
                }
            }
            else
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                    this.Refresh();
                }
                else
                {
                    Close();
                }
            }
        }

        public ImageEx GetScreenshot()
        {
            Rect rect = SelectionRectangle0Based;

            if (rect.Width > 0 && rect.Height > 0)
            {
                if (rect.X == 0 && rect.Y == 0 && rect.Width == backgroundImage.Width && rect.Height == backgroundImage.Height)
                {
                    return new ImageEx(backgroundImage.Clone());
                }

                return new ImageEx(ImageHelper.CropImage(backgroundImage, rect));
            }

            return null;
        }
    }
}