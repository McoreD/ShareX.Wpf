using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxAccountInfo
    {
        public string Referral_link { get; set; } // The user's referral link.
        public string Display_name { get; set; } // The user's display name.
        public long Uid { get; set; } // The user's unique Dropbox ID.
        public string Country { get; set; } // The user's two-letter country code, if available.
        public DropboxQuotaInfo Quota_info { get; set; }
        public string Email { get; set; }
    }
}