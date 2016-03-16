using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.Dropbox
{
    public class DropboxQuotaInfo
    {
        public long Normal { get; set; } // The user's used quota outside of shared folders (bytes).
        public long Shared { get; set; } // The user's used quota in shared folders (bytes).
        public long Quota { get; set; } // The user's total quota allocation (bytes).
    }
}