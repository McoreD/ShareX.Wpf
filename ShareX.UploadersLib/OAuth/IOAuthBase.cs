using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public interface IOAuthBase
    {
        string GetAuthorizationURL();

        bool GetAccessToken(string code);
    }
}