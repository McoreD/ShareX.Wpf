using HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX.UploadersLib
{
    public class UploaderPluginsManager
    {
        public Dictionary<string, IShareXUploaderPlugin> Plugins { get; private set; }

        public UploaderPluginsManager(string folderPath)
        {
            Plugins = PluginHelper<IShareXUploaderPlugin>.LoadPlugins(folderPath);

            if (Plugins != null)
            {
                foreach (var plugin in Plugins)
                {
                    IShareXUploaderPlugin uploader = plugin.Value;
                    uploader.Location = plugin.Key;
                    uploader.LoadSettings(Path.ChangeExtension(uploader.Location, "json"));
                }
            }
        }

        public void SaveSettings()
        {
            if (Plugins != null)
            {
                foreach (var plugin in Plugins)
                {
                    plugin.Value.SaveSettings();
                }
            }
        }
    }
}