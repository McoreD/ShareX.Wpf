using System;
using System.Windows.Controls;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurUploader : IShareXUploaderPlugin
    {
        public string Name { get; set; } = "Imgur";
        public string Publisher { get; } = "ShareX Team";
        public string Location { get; set; }

        private ImgurSettings config = new ImgurSettings();

        public UploaderSettings Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value as ImgurSettings;
            }
        }

        public UserControl UI
        {
            get
            {
                return new ImgurControl();
            }
        }

        public void LoadSettings(string filePath)
        {
            //  throw new NotImplementedException();
        }

        public void SaveSettings()
        {
            // throw new NotImplementedException();
        }
    }
}