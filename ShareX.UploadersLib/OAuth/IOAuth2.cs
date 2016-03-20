using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public interface IOAuth2 : IOAuth2Basic
    {
        bool RefreshAccessToken();

        bool CheckAuthorization();
    }
}