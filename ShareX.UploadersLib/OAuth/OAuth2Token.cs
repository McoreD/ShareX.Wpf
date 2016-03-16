using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public class OAuth2Token
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }

        public DateTime ExpireDate { get; set; }

        [JsonIgnore]
        public bool IsExpired
        {
            get
            {
                return ExpireDate == DateTime.MinValue || DateTime.UtcNow > ExpireDate;
            }
        }

        public void UpdateExpireDate()
        {
            ExpireDate = DateTime.UtcNow + TimeSpan.FromSeconds(expires_in - 10);
        }
    }
}