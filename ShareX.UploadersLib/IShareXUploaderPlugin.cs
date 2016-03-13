using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShareX.UploadersLib
{
    public interface IShareXUploaderPlugin
    {
        string Name { get; }
        UserControl UI { get; }
        UploaderConfig Config { get; set; }
    }
}