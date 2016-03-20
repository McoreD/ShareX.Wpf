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
        public delegate void PluginsLoadedEventHandler();
        public event PluginsLoadedEventHandler PluginsLoaded;
        public Dictionary<string, IShareXUploaderPlugin> Plugins { get; private set; }

        public void Init(string folderPath)
        {
            Plugins = PluginHelper<IShareXUploaderPlugin>.LoadPlugins(folderPath);

            OnPluginsLoaded();

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

        protected virtual void OnPluginsLoaded()
        {
            if (PluginsLoaded != null) PluginsLoaded();
        }

        public IShareXUploaderPlugin GetUploader(string name)
        {
            foreach (var plugin in Plugins)
            {
                if (plugin.Value.Name == name)
                    return plugin.Value;
            }

            return null;
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