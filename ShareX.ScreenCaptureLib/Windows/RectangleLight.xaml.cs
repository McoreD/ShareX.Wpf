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
            backgroundImage = ScreenshotHelper.CaptureFullscreen();
            Background = new ImageBrush(backgroundImage);

            ScreenRectangle = CaptureHelpers.GetScreenBounds();

            InitializeComponent();

            CropArea.Stroke = Brushes.Red;
            CropArea.Fill = new SolidColorBrush() { Color = Color.FromArgb(50, 132, 112, 255) };
        }

        protected override void OnInitialized(EventArgs e)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Bounds(ScreenRectangle);

            this.WindowStyle = WindowStyle.None;
            this.ShowInTaskbar = false;
            this.Topmost = true;

            base.OnInitialized(e);
        }

        protected override void OnContentRendered(EventArgs e)
        {
            ShowActivated = true;
            base.OnContentRendered(e);
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                Close();
            }

            canvas.Children.Add(CropArea);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                currentPosition = CaptureHelpers.GetCursorPosition();
                SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, currentPosition.X, currentPosition.Y);

                CropArea.SetValue(Canvas.LeftProperty, Math.Min(currentPosition.X, positionOnClick.X));
                CropArea.SetValue(Canvas.TopProperty, Math.Min(currentPosition.Y, positionOnClick.Y));
                CropArea.Width = Math.Abs(currentPosition.X - positionOnClick.X);
                CropArea.Height = Math.Abs(currentPosition.Y - positionOnClick.Y);
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        public BitmapSource GetAreaImage()
        {
            Rect rect = SelectionRectangle0Based;

            if (rect.Width > 0 && rect.Height > 0)
            {
                if (rect.X == 0 && rect.Y == 0 && rect.Width == backgroundImage.Width && rect.Height == backgroundImage.Height)
                {
                    return backgroundImage.Clone();
                }

                return ImageHelpers.CropImage(backgroundImage, rect);
            }

            return null;
        }
    }
}