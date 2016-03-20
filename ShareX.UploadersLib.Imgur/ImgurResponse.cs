using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurResponse
    {
        public object data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }
}