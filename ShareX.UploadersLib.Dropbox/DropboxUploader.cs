using System.Windows.Controls;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxUploader : IShareXUploaderPlugin
    {
        private DropboxConfig config = new DropboxConfig();

        public UploaderConfig Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value as DropboxConfig;
            }
        }

        public string Name
        {
            get
            {
                return "Dropbox";
            }
        }

        public UserControl UI
        {
            get
            {
                return new DropboxControl();
            }
        }
    }
}