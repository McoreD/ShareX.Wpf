using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public abstract class FileUploader : Uploader
    {
        public abstract UploadResult Upload(Stream stream, string fileName);

        public UploadResult Upload(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return Upload(stream, Path.GetFileName(filePath));
                }
            }

            return null;
        }
    }
}