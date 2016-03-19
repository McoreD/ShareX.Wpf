using HelpersLib;
using ShareX.UploadersLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ShareX
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsMultiInstance { get; private set; }
        public static bool IsPortable { get; private set; }
        public static bool IsPortableApps { get; private set; }
        public static bool IsSilentRun { get; private set; }
        public static bool IsSandbox { get; private set; }
        public static bool IsFirstTimeConfig { get; private set; }
        public static bool NoHotkeys { get; private set; }

        public static Settings Settings { get; private set; }

        #region Paths

        public static readonly string DefaultPersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ShareX.Wpf");
        private static readonly string PortablePersonalFolder = Helper.GetAbsolutePath("ShareX.Wpf");
        private static readonly string PortableAppsPersonalFolder = Helper.GetAbsolutePath("../../Data");
        private static readonly string PersonalPathConfigFilePath = Helper.GetAbsolutePath("PersonalPath.cfg");
        private static readonly string PortableCheckFilePath = Helper.GetAbsolutePath("Portable");
        private static readonly string PortableAppsCheckFilePath = Helper.GetAbsolutePath("PortableApps");
        public static readonly string ChromeHostFilePath = Helper.GetAbsolutePath("ShareX_Chrome.exe");
        public static readonly string SteamInAppFilePath = Helper.GetAbsolutePath("Steam");

        private static string CustomPersonalPath { get; set; }

        public static string PersonalFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(CustomPersonalPath))
                {
                    return Helper.ExpandFolderVariables(CustomPersonalPath);
                }

                return DefaultPersonalFolder;
            }
        }

        public static string ApplicationConfigFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(PersonalFolder, "AppSettings.json");
                }

                return null;
            }
        }

        public static string UploadersFolderPath
        {
            get
            {
                if (!IsSandbox)
                {
                    string uploadersConfigFolder;

                    if (Settings != null && !string.IsNullOrEmpty(Settings.CustomUploadersFolder))
                    {
                        uploadersConfigFolder = Helper.ExpandFolderVariables(Settings.CustomUploadersFolder);
                    }
                    else
                    {
                        uploadersConfigFolder = "Plugins"; // Path.Combine(PersonalFolder, "Uploaders");
                    }

                    return uploadersConfigFolder;
                }

                return null;
            }
        }

        public static string HotkeysConfigFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    string hotkeysConfigFolder;

                    if (Settings != null && !string.IsNullOrEmpty(Settings.CustomHotkeysConfigPath))
                    {
                        hotkeysConfigFolder = Helper.ExpandFolderVariables(Settings.CustomHotkeysConfigPath);
                    }
                    else
                    {
                        hotkeysConfigFolder = PersonalFolder;
                    }

                    return Path.Combine(hotkeysConfigFolder, "HotkeysConfig.json");
                }

                return null;
            }
        }

        public static string HistoryFilePath
        {
            get
            {
                if (!IsSandbox)
                {
                    return Path.Combine(PersonalFolder, "History.xml");
                }

                return null;
            }
        }

        public static string LogsFolder => Path.Combine(PersonalFolder, "Logs");

        public static string LogsFilePath
        {
            get
            {
                string filename = string.Format("ShareX-Log-{0:yyyy-MM}.txt", DateTime.Now);
                return Path.Combine(LogsFolder, filename);
            }
        }

        public static string ScreenshotsParentFolder
        {
            get
            {
                if (Settings != null && Settings.UseCustomScreenshotsPath && !string.IsNullOrEmpty(Settings.CustomScreenshotsPath))
                {
                    return Helper.ExpandFolderVariables(Settings.CustomScreenshotsPath);
                }

                return Path.Combine(PersonalFolder, "Screenshots");
            }
        }

        public static string ScreenshotsFolder
        {
            get
            {
                string subFolderName = NameParser.Parse(NameParserType.FolderPath, Settings.SaveImageSubFolderPattern);
                return Path.Combine(ScreenshotsParentFolder, subFolderName);
            }
        }

        public static string BackupFolder => Path.Combine(PersonalFolder, "Backup");
        public static string ToolsFolder => Path.Combine(PersonalFolder, "Tools");
        public static string GreenshotImageEditorConfigFilePath => Path.Combine(PersonalFolder, "GreenshotImageEditor.ini");
        public static string ScreenRecorderCacheFilePath => Path.Combine(PersonalFolder, "ScreenRecorder.avi");
        public static string DefaultFFmpegFilePath => Path.Combine(ToolsFolder, "ffmpeg.exe");
        public static string ChromeHostManifestFilePath => Path.Combine(ToolsFolder, "Chrome-host-manifest.json");

        #endregion Paths
    }
}