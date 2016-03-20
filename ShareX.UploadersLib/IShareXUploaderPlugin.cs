using HelpersLib;
using System.IO;
using System.Windows.Controls;

namespace ShareX.UploadersLib
{
    public interface IShareXUploaderPlugin : IPluginBase
    {
        string Location { get; set; }

        UserControl UI { get; }

        void LoadSettings(string filePath);

        UploadResult Upload(Stream stream, string fileName);

        void SaveSettings();
    }
}