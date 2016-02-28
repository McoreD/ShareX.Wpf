using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ShareX.ScreenCaptureLib
{
    public class Screenshot : Capture
    {
        public ImageSource Source { get; private set; }
        public ObservableCollection<Highlight> Highlights { get; set; } = new ObservableCollection<Highlight>();

        public Screenshot(ImageSource src)
        {
            Source = src;
        }
    }
}