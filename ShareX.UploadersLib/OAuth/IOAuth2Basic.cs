using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public interface IOAuth2Basic : IOAuthBase
    {
        OAuth2Info AuthInfo { get; }
    }
}