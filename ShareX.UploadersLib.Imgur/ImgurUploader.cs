using System.Windows.Controls;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurUploader : IShareXUploaderPlugin
    {
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

        public string Name
        {
            get
            {
                return "Imgur";
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