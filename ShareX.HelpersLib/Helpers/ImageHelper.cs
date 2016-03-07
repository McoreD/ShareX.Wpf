using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HelpersLib
{
    public static class ImageHelper
    {
        public static BitmapSource CropImage(BitmapSource img, Rect rect)
        {
            if (img != null && rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0 && new Rect(0, 0, (int)img.Width, (int)img.Height).Contains(rect))
            {
                return new CroppedBitmap(img, new Int32Rect((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height));
            }

            return null;
        }

        public static bool IsEqual(this BitmapSource image1, BitmapSource image2)
        {
            if (image1 == null || image2 == null)
            {
                return false;
            }
            return image1.ToBytes().SequenceEqual(image2.ToBytes());
        }

        public static byte[] ToBytes(this BitmapSource image)
        {
            byte[] data = new byte[] { };
            if (image != null)
            {
                try
                {
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        data = ms.ToArray();
                    }
                    return data;
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }
            }
            return data;
        }
    }
}