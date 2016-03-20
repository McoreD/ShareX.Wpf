using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.Imgur
{
    public enum ImgurThumbnailType
    {
        [Description("Small square")]
        Small_Square,
        [Description("Big square")]
        Big_Square,
        [Description("Small thumbnail")]
        Small_Thumbnail,
        [Description("Medium thumbnail")]
        Medium_Thumbnail,
        [Description("Large thumbnail")]
        Large_Thumbnail,
        [Description("Huge thumbnail")]
        Huge_Thumbnail
    }
}