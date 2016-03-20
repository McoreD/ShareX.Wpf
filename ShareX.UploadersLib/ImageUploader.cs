using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ShareX.UploadersLib
{
    public abstract class ImageUploader : FileUploader
    {
        public UploadResult Upload(BitmapSource image, string fileName)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return Upload(stream, fileName);
            }
        }
    }
}