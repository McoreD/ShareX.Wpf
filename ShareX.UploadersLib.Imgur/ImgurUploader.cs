using System;
using System.Windows.Controls;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurUploader : IShareXUploaderPlugin
    {
        public string Name { get; set; } = "Imgur";
        public string Publisher { get; } = "ShareX Team";

        private ImgurConfig config = new ImgurConfig();

        public UploaderConfig Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value as ImgurConfig;
            }
        }

        public UserControl UI
        {
            get
            {
                return new ImgurControl();
            }
        }
    }
}