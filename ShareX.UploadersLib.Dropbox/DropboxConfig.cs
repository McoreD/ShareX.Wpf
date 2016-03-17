using HelpersLib;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxConfig : UploaderConfig
    {
        public OAuth2Info DropboxOAuth2Info = null;
        public DropboxAccountInfo DropboxAccountInfo = null;
        public string DropboxUploadPath = "Public/ShareX/%y/%mo";
        public bool DropboxAutoCreateShareableLink = false;
        public DropboxURLType DropboxURLType = DropboxURLType.Default;
    }
}