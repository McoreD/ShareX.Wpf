using HelpersLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Controls;

namespace ShareX.UploadersLib.Imgur
{
    public class ImgurUploader : ImageUploader, IShareXUploaderPlugin, IOAuth2
    {
        public string Name { get; set; } = "Imgur";
        public string Publisher { get; } = "ShareX Team";
        public string Location { get; set; }

        public static ImgurSettings Config { get; set; }

        public AccountType UploadMethod { get; set; }
        public OAuth2Info AuthInfo { get; set; }
        public ImgurThumbnailType ThumbnailType { get; set; }
        public string UploadAlbumID { get; set; }
        public bool UseGIFV { get; set; }

        public UserControl UI
        {
            get
            {
                return new ImgurControl();
            }
        }

        public ImgurUploader()
        {
        }

        public ImgurUploader(OAuth2Info oauth)
        {
            AuthInfo = oauth;
        }

        public void LoadSettings(string filePath)
        {
            Config = ImgurSettings.Load(filePath) as ImgurSettings;
            AuthInfo = Config.ImgurOAuth2Info;
        }

        public void SaveSettings()
        {
            Config.Save();
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("response_type", "pin");

            return CreateQuery("https://api.imgur.com/oauth2/authorize", args);
        }

        public bool GetAccessToken(string pin)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "pin");
            args.Add("pin", pin);

            string response = SendRequest(HttpMethod.POST, "https://api.imgur.com/oauth2/token", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("refresh_token", AuthInfo.Token.refresh_token);
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("grant_type", "refresh_token");

                string response = SendRequest(HttpMethod.POST, "https://api.imgur.com/oauth2/token", args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        AuthInfo.Token = token;
                        return true;
                    }
                }
            }

            return false;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !RefreshAccessToken())
                {
                    Errors.Add("Refresh access token failed.");
                    return false;
                }
            }
            else
            {
                Errors.Add("Imgur login is required.");
                return false;
            }

            return true;
        }

        public List<ImgurAlbumData> GetAlbums()
        {
            if (CheckAuthorization())
            {
                string response = SendRequest(HttpMethod.GET, "https://api.imgur.com/3/account/me/albums", headers: GetAuthHeaders());

                ImgurResponse imgurResponse = JsonConvert.DeserializeObject<ImgurResponse>(response);

                if (imgurResponse != null)
                {
                    if (imgurResponse.success && imgurResponse.status == 200)
                    {
                        return ((JArray)imgurResponse.data).ToObject<List<ImgurAlbumData>>();
                    }
                    else
                    {
                        HandleErrors(imgurResponse);
                    }
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            return InternalUpload(stream, fileName, true);
        }

        private UploadResult InternalUpload(Stream stream, string fileName, bool refreshTokenOnError)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            NameValueCollection headers;

            if (UploadMethod == AccountType.User)
            {
                if (!CheckAuthorization())
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(UploadAlbumID))
                {
                    args.Add("album", UploadAlbumID);
                }

                headers = GetAuthHeaders();
            }
            else
            {
                headers = new NameValueCollection();
                headers.Add("Authorization", "Client-ID " + AuthInfo.Client_ID);
            }

            WebExceptionReturnResponse = true;
            UploadResult result = UploadData(stream, "https://api.imgur.com/3/image", fileName, "image", args, headers);

            if (!string.IsNullOrEmpty(result.Response))
            {
                ImgurResponse imgurResponse = JsonConvert.DeserializeObject<ImgurResponse>(result.Response);

                if (imgurResponse != null)
                {
                    if (imgurResponse.success && imgurResponse.status == 200)
                    {
                        ImgurImageData imageData = ((JObject)imgurResponse.data).ToObject<ImgurImageData>();

                        if (imageData != null && !string.IsNullOrEmpty(imageData.link))
                        {
                            if (Config.DirectLink)
                            {
                                if (UseGIFV && !string.IsNullOrEmpty(imageData.gifv))
                                {
                                    result.URL = imageData.gifv;
                                }
                                else
                                {
                                    result.URL = imageData.link;
                                }
                            }
                            else
                            {
                                result.URL = "http://imgur.com/" + imageData.id;
                            }

                            string thumbnail = string.Empty;

                            switch (ThumbnailType)
                            {
                                case ImgurThumbnailType.Small_Square:
                                    thumbnail = "s";
                                    break;
                                case ImgurThumbnailType.Big_Square:
                                    thumbnail = "b";
                                    break;
                                case ImgurThumbnailType.Small_Thumbnail:
                                    thumbnail = "t";
                                    break;
                                case ImgurThumbnailType.Medium_Thumbnail:
                                    thumbnail = "m";
                                    break;
                                case ImgurThumbnailType.Large_Thumbnail:
                                    thumbnail = "l";
                                    break;
                                case ImgurThumbnailType.Huge_Thumbnail:
                                    thumbnail = "h";
                                    break;
                            }

                            result.ThumbnailURL = string.Format("http://i.imgur.com/{0}{1}.jpg", imageData.id, thumbnail); // Thumbnails always jpg
                            result.DeletionURL = "http://imgur.com/delete/" + imageData.deletehash;
                        }
                    }
                    else
                    {
                        ImgurErrorData errorData = ((JObject)imgurResponse.data).ToObject<ImgurErrorData>();

                        if (errorData != null)
                        {
                            if (UploadMethod == AccountType.User && refreshTokenOnError &&
                                errorData.error.Equals("The access token provided is invalid.", StringComparison.InvariantCultureIgnoreCase) && RefreshAccessToken())
                            {
                                DebugHelper.WriteLine("Imgur access token refreshed, reuploading image.");

                                return InternalUpload(stream, fileName, false);
                            }

                            string errorMessage = string.Format("Imgur upload failed: ({0}) {1}", imgurResponse.status, errorData.error);
                            Errors.Clear();
                            Errors.Add(errorMessage);
                        }
                    }
                }
            }

            return result;
        }

        private void HandleErrors(ImgurResponse response)
        {
            ImgurErrorData errorData = ((JObject)response.data).ToObject<ImgurErrorData>();

            if (errorData != null)
            {
                string errorMessage = string.Format("Status: {0}, Request: {1}, Error: {2}", response.status, errorData.request, errorData.error);
                Errors.Add(errorMessage);
            }
        }
    }
}