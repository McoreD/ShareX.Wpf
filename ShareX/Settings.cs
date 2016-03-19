using HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareX
{
    public class Settings : SettingsBase<Settings>
    {
        #region Paths

        public bool UseCustomScreenshotsPath = false;
        public string CustomScreenshotsPath = string.Empty;
        public string SaveImageSubFolderPattern = "%y-%mo";

        #endregion Paths

        [Category("Paths"), Description("Custom uploaders configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        public string CustomUploadersConfigPath { get; set; }

        [Category("Paths"), Description("Custom hotkeys configuration path. If you have already configured this setting in another device and you are attempting to use the same location, then backup the file before configuring this setting and restore after exiting ShareX.")]
        public string CustomHotkeysConfigPath { get; set; }
    }
}