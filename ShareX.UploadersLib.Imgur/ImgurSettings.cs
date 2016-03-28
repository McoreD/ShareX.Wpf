using HelpersLib;
using System.Collections.Generic;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurSettings : SettingsBase<ImgurSettings>
    {
        public AccountType ImgurAccountType = AccountType.Anonymous;
        public bool DirectLink = true;
        public ImgurThumbnailType ImgurThumbnailType = ImgurThumbnailType.Large_Thumbnail;
        public bool ImgurUseGIFV = true;
        public OAuth2Info ImgurOAuth2Info = null;
        public bool ImgurUploadSelectedAlbum = false;
        public ImgurAlbumData ImgurSelectedAlbum = null;
        public List<ImgurAlbumData> ImgurAlbumList = null;
    }
}