﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public class OAuthInfo : ICloneable
    {
        public enum OAuthInfoSignatureMethod
        {
            HMAC_SHA1,
            RSA_SHA1
        }

        public string Description { get; set; }

        [Browsable(false)]
        public string OAuthVersion { get; set; }

        [Browsable(false)]
        public string ConsumerKey { get; set; }

        // Used for HMAC_SHA1 signature
        [Browsable(false)]
        public string ConsumerSecret { get; set; }

        // Used for RSA_SHA1 signature
        [Browsable(false)]
        public string ConsumerPrivateKey { get; set; }

        [Browsable(false)]
        public OAuthInfoSignatureMethod SignatureMethod { get; set; }

        [Browsable(false)]
        public string AuthToken { get; set; }

        [Browsable(false)]
        public string AuthSecret { get; set; }

        [Description("Verification Code from the Authorization Page")]
        public string AuthVerifier { get; set; }

        [Browsable(false)]
        public string UserToken { get; set; }

        [Browsable(false)]
        public string UserSecret { get; set; }

        public OAuthInfo()
        {
            Description = "New account";
            OAuthVersion = "1.0";
        }

        public OAuthInfo(string consumerKey)
            : this()
        {
            ConsumerKey = consumerKey;
        }

        public OAuthInfo(string consumerKey, string consumerSecret)
            : this()
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public OAuthInfo(string consumerKey, string consumerSecret, string userToken, string userSecret)
            : this(consumerKey, consumerSecret)
        {
            UserToken = userToken;
            UserSecret = userSecret;
        }

        public static bool CheckOAuth(OAuthInfo oauth)
        {
            return oauth != null && !string.IsNullOrEmpty(oauth.ConsumerKey) &&
                ((!string.IsNullOrEmpty(oauth.ConsumerSecret) && oauth.SignatureMethod == OAuthInfoSignatureMethod.HMAC_SHA1)
                || oauth.ConsumerPrivateKey != null && oauth.SignatureMethod == OAuthInfoSignatureMethod.RSA_SHA1)
                && !string.IsNullOrEmpty(oauth.UserToken) && !string.IsNullOrEmpty(oauth.UserSecret);
        }

        public OAuthInfo Clone()
        {
            return MemberwiseClone() as OAuthInfo;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
            return Description;
        }
    }
}