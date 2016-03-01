using HelpersLib;
using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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

        private Timer timer;
        private BitmapSource backgroundImage;
        private ImageBrush backgroundBrush;
        private Pen borderDotPen, borderDotPen2;
        private Point currentPosition, positionOnClick;
        private bool isMouseDown;
        private Stopwatch penTimer;

        public RectangleLight()
        {
            backgroundImage = ScreenshotHelper.CaptureFullscreen();
            backgroundBrush = new ImageBrush(backgroundImage);
            this.Background = backgroundBrush;

            borderDotPen = new Pen(Brushes.Black, 1);
            borderDotPen2 = new Pen(Brushes.White, 1);
            borderDotPen2.DashStyle = DashStyles.Dash;
            penTimer = Stopwatch.StartNew();
            ScreenRectangle = CaptureHelpers.GetScreenBounds();

            InitializeComponent();

            //using (MemoryStream cursorStream = new MemoryStream((byte)TryFindResource("Crosshair.cur")))
            //{
            //    Cursor = new Cursor(cursorStream);
            //}

            timer = new Timer { Interval = 10 };
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

            this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
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

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (timer != null) timer.Dispose();
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
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                positionOnClick = CaptureHelpers.GetCursorPosition();
                isMouseDown = true;
            }
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Released)
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentPosition = CaptureHelpers.GetCursorPosition();
            SelectionRectangle = CaptureHelpers.CreateRectangle(positionOnClick.X, positionOnClick.Y, currentPosition.X, currentPosition.Y);

            this.Refresh();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (isMouseDown)
            {
                borderDotPen2.DashStyle.Offset = (float)penTimer.Elapsed.TotalSeconds * -15;
                drawingContext.DrawRectangle(backgroundBrush, borderDotPen, SelectionRectangle0Based);
                drawingContext.DrawRectangle(backgroundBrush, borderDotPen2, SelectionRectangle0Based);

                base.OnRender(drawingContext);
            }
        }
    }
}