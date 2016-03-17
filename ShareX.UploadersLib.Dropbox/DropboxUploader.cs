using HelpersLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxUploader : FileUploader, IShareXUploaderPlugin, IOAuth2Basic
    {
        public string Name { get; set; } = "Dropbox";
        public string Publisher { get; } = "ShareX Team";

        public OAuth2Info AuthInfo { get; set; }
        private DropboxAccountInfo AccountInfo { get; set; }
        public string UploadPath { get; set; }
        public bool AutoCreateShareableLink { get; set; }
        public DropboxURLType ShareURLType { get; set; }

        private const string APIVersion = "1";
        private const string Root = "dropbox"; // dropbox or sandbox

        private const string URLWEB = "https://www.dropbox.com/" + APIVersion;
        private const string URLAPI = "https://api.dropbox.com/" + APIVersion;
        private const string URLAPIContent = "https://api-content.dropbox.com/" + APIVersion;

        private const string URLAccountInfo = URLAPI + "/account/info";
        private const string URLFiles = URLAPIContent + "/files/" + Root;
        private const string URLMetaData = URLAPI + "/metadata/" + Root;
        private const string URLShares = URLAPI + "/shares/" + Root;
        private const string URLCopy = URLAPI + "/fileops/copy";
        private const string URLCreateFolder = URLAPI + "/fileops/create_folder";
        private const string URLDelete = URLAPI + "/fileops/delete";
        private const string URLMove = URLAPI + "/fileops/move";
        private const string URLPublicDirect = "https://dl.dropboxusercontent.com/u";
        private const string URLShareDirect = "https://dl.dropboxusercontent.com/s";

        public UserControl UI
        {
            get
            {
                return new DropboxControl();
            }
        }

        public DropboxUploader()
        {
        }

        public DropboxUploader(OAuth2Info oauth)
            : this()
        {
            AuthInfo = oauth;
        }

        // https://www.dropbox.com/developers/core/docs#oa2-authorize
        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("response_type", "code");
            args.Add("client_id", AuthInfo.Client_ID);

            return CreateQuery(URLWEB + "/oauth2/authorize", args);
        }

        // https://www.dropbox.com/developers/core/docs#oa2-token
        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("grant_type", "authorization_code");
            args.Add("code", code);

            string response = SendRequest(HttpMethod.POST, URLAPI + "/oauth2/token", args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public string GetPublicURL(string path)
        {
            return GetPublicURL(AccountInfo.Uid, path);
        }

        public static string GetPublicURL(long userID, string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim('/');

                if (path.StartsWith("Public/", StringComparison.InvariantCultureIgnoreCase))
                {
                    path = URLHelper.URLPathEncode(path.Substring(7));
                    return URLHelper.CombineURL(URLPublicDirect, userID.ToString(), path);
                }
            }

            return "Upload path is private. Use \"Public\" folder to get public URL.";
        }

        private void CheckEarlyURLCopy(string path, string fileName)
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !AutoCreateShareableLink)
            {
                string url = GetPublicURL(URLHelper.CombineURL(path, fileName));
                OnEarlyURLCopyRequested(url);
            }
        }

        // https://www.dropbox.com/developers/core/docs#shares
        public string CreateShareableLink(string path, DropboxURLType urlType)
        {
            if (!string.IsNullOrEmpty(path) && OAuth2Info.CheckOAuth(AuthInfo))
            {
                string url = URLHelper.CombineURL(URLShares, URLHelper.URLPathEncode(path));

                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("short_url", urlType == DropboxURLType.Shortened ? "true" : "false");

                string response = SendRequest(HttpMethod.POST, url, args, GetAuthHeaders());

                if (!string.IsNullOrEmpty(response))
                {
                    DropboxShares shares = JsonConvert.DeserializeObject<DropboxShares>(response);

                    if (urlType == DropboxURLType.Direct)
                    {
                        Match match = Regex.Match(shares.URL, @"https?://(?:www\.)?dropbox.com/s/(?<path>\w+/.+)");
                        if (match.Success)
                        {
                            string urlPath = match.Groups["path"].Value;
                            if (!string.IsNullOrEmpty(urlPath))
                            {
                                return URLHelper.CombineURL(URLShareDirect, urlPath);
                            }
                        }
                    }
                    else
                    {
                        return shares.URL;
                    }
                }
            }

            return null;
        }

        private NameValueCollection GetAuthHeaders()
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Authorization", "Bearer " + AuthInfo.Token.access_token);
            return headers;
        }

        // https://www.dropbox.com/developers/core/docs#files_put
        public UploadResult UploadFile(Stream stream, string path, string fileName, bool createShareableURL = false, DropboxURLType urlType = DropboxURLType.Default)
        {
            if (!OAuth2Info.CheckOAuth(AuthInfo))
            {
                Errors.Add("Dropbox login is required.");
                return null;
            }

            string url = URLHelper.CombineURL(URLFiles, URLHelper.URLPathEncode(path));

            // There's a 150MB limit to all uploads through the API.
            UploadResult result = UploadData(stream, url, fileName, headers: GetAuthHeaders());

            if (result.IsSuccess)
            {
                DropboxContentInfo content = JsonConvert.DeserializeObject<DropboxContentInfo>(result.Response);

                if (content != null)
                {
                    if (createShareableURL)
                    {
                        AllowReportProgress = false;
                        result.URL = CreateShareableLink(content.Path, urlType);
                    }
                    else
                    {
                        result.URL = GetPublicURL(content.Path);
                    }
                }
            }

            return result;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            CheckEarlyURLCopy(UploadPath, fileName);

            return UploadFile(stream, UploadPath, fileName, AutoCreateShareableLink, ShareURLType);
        }
    }
}