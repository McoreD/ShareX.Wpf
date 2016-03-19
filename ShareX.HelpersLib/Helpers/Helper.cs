using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace HelpersLib
{
    public static class Helper
    {
        public const string Numbers = "0123456789"; // 48 ... 57
        public const string AlphabetCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // 65 ... 90
        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz"; // 97 ... 122
        public const string Alphanumeric = Numbers + AlphabetCapital + Alphabet;
        public const string AlphanumericInverse = Numbers + Alphabet + AlphabetCapital;
        public const string Hexadecimal = Numbers + "ABCDEF";
        public const string URLCharacters = Alphanumeric + "-._~"; // 45 46 95 126
        public const string URLPathCharacters = URLCharacters + "/"; // 47
        public const string ValidURLCharacters = URLPathCharacters + ":?#[]@!$&'()*+,;= ";

        public static readonly string[] ImageFileExtensions = new string[] { "jpg", "jpeg", "png", "gif", "bmp", "ico", "tif", "tiff" };
        public static readonly string[] TextFileExtensions = new string[] { "txt", "log", "nfo", "c", "cpp", "cc", "cxx", "h", "hpp", "hxx", "cs", "vb", "html", "htm", "xhtml", "xht", "xml", "css", "js", "php", "bat", "java", "lua", "py", "pl", "cfg", "ini" };

        public static readonly Version OSVersion = Environment.OSVersion.Version;

        public static string BrowseFolder(string title = "Browse folder")
        {
            FolderDialog dlg = new FolderDialog();

            if (dlg.ShowDialog())
            {
                return dlg.Path;
            }

            return null;
        }

        public static void CreateDirectoryFromDirectoryPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }

        public static void CreateDirectoryFromFilePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                CreateDirectoryFromDirectoryPath(Path.GetDirectoryName(path));
            }
        }

        public static T[] GetEnums<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static string[] GetEnumDescriptions<T>()
        {
            return Enum.GetValues(typeof(T)).OfType<Enum>().Select(x => x.GetDescription()).ToArray();
        }

        #region OS version checks

        public static bool IsWindowsXP()
        {
            return OSVersion.Major == 5 && OSVersion.Minor == 1;
        }

        public static bool IsWindowsXPOrGreater()
        {
            return (OSVersion.Major == 5 && OSVersion.Minor >= 1) || OSVersion.Major > 5;
        }

        public static bool IsWindowsVista()
        {
            return OSVersion.Major == 6;
        }

        public static bool IsWindowsVistaOrGreater()
        {
            return OSVersion.Major >= 6;
        }

        public static bool IsWindows7()
        {
            return OSVersion.Major == 6 && OSVersion.Minor == 1;
        }

        public static bool IsWindows7OrGreater()
        {
            return (OSVersion.Major == 6 && OSVersion.Minor >= 1) || OSVersion.Major > 6;
        }

        public static bool IsWindows8()
        {
            return OSVersion.Major == 6 && OSVersion.Minor == 2;
        }

        public static bool IsWindows8OrGreater()
        {
            return (OSVersion.Major == 6 && OSVersion.Minor >= 2) || OSVersion.Major > 6;
        }

        public static bool IsWindows10OrGreater()
        {
            return OSVersion.Major >= 10;
        }

        #endregion OS version checks

        public static string GetMimeType(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLower();

                if (!string.IsNullOrEmpty(ext))
                {
                    string mimeType = MimeTypes.GetMimeType(ext);

                    if (!string.IsNullOrEmpty(mimeType))
                    {
                        return mimeType;
                    }

                    using (RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(ext))
                    {
                        if (regKey != null && regKey.GetValue("Content Type") != null)
                        {
                            mimeType = regKey.GetValue("Content Type").ToString();

                            if (!string.IsNullOrEmpty(mimeType))
                            {
                                return mimeType;
                            }
                        }
                    }
                }
            }

            return MimeTypes.DefaultMimeType;
        }

        public static string AddZeroes(string input, int digits = 2)
        {
            return input.PadLeft(digits, '0');
        }

        public static string AddZeroes(int number, int digits = 2)
        {
            return AddZeroes(number.ToString(), digits);
        }

        public static string HourTo12(int hour)
        {
            if (hour == 0)
            {
                return 12.ToString();
            }

            if (hour > 12)
            {
                return AddZeroes(hour - 12);
            }

            return AddZeroes(hour);
        }

        public static string RepeatGenerator(int count, Func<string> generator)
        {
            string result = "";
            for (int x = count; x > 0; x--)
            {
                result += generator();
            }
            return result;
        }

        public static DateTime UnixToDateTime(long unix)
        {
            long timeInTicks = unix * TimeSpan.TicksPerSecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
        }

        public static long DateTimeToUnix(DateTime dateTime)
        {
            DateTime date = dateTime.ToUniversalTime();
            long ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            return ticks / TimeSpan.TicksPerSecond;
        }

        public static char GetRandomChar(string chars)
        {
            return chars[MathHelper.Random(chars.Length - 1)];
        }

        public static string GetRandomString(string chars, int length)
        {
            StringBuilder sb = new StringBuilder();

            while (length-- > 0)
            {
                sb.Append(GetRandomChar(chars));
            }

            return sb.ToString();
        }

        public static string GetRandomNumber(int length)
        {
            return GetRandomString(Numbers, length);
        }

        public static string GetRandomAlphanumeric(int length)
        {
            return GetRandomString(Alphanumeric, length);
        }

        #region ProductionVersion

        private static Version GetProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }

        public static Version ProductVersion
        {
            get
            {
                return GetProductVersion();
            }
        }

        private static Version NormalizeVersion(string version)
        {
            return Version.Parse(version).Normalize();
        }

        /// <summary>
        /// If version1 newer than version2 = 1
        /// If version1 equal to version2 = 0
        /// If version1 older than version2 = -1
        /// </summary>
        public static int CompareVersion(string version1, string version2)
        {
            return NormalizeVersion(version1).CompareTo(NormalizeVersion(version2));
        }

        /// <summary>
        /// If version1 newer than version2 = 1
        /// If version1 equal to version2 = 0
        /// If version1 older than version2 = -1
        /// </summary>
        public static int CompareVersion(Version version1, Version version2)
        {
            return version1.Normalize().CompareTo(version2.Normalize());
        }

        /// <summary>
        /// If version newer than ApplicationVersion = 1
        /// If version equal to ApplicationVersion = 0
        /// If version older than ApplicationVersion = -1
        /// </summary>
        public static int CompareApplicationVersion(string version)
        {
            return CompareVersion(version, ProductVersion.ToString());
        }

        #endregion ProductionVersion

        public static string ExpandFolderVariables(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    // TODO: folderPath = folderPath.Replace($"%ShareX%", Program.ToolsFolder, StringComparison.InvariantCultureIgnoreCase));
                    GetEnums<Environment.SpecialFolder>().ToList<Environment.SpecialFolder>().ForEach(x => path = path.Replace($"%{x}%", Environment.GetFolderPath(x), StringComparison.InvariantCultureIgnoreCase));
                    path = Environment.ExpandEnvironmentVariables(path);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return path;
        }

        public static string GetValidFileName(string fileName, string separator = "")
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            if (string.IsNullOrEmpty(separator))
            {
                return new string(fileName.Where(c => !invalidFileNameChars.Contains(c)).ToArray());
            }
            else
            {
                invalidFileNameChars.ToList<char>().ForEach(x => fileName = fileName.Replace(x.ToString(), separator));
                return fileName.Trim().Replace(separator + separator, separator);
            }
        }

        public static string GetValidFolderPath(string folderPath)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            return new string(folderPath.Where(c => !invalidPathChars.Contains(c)).ToArray());
        }

        public static string GetValidFilePath(string filePath)
        {
            string folderPath = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            return GetValidFolderPath(folderPath) + Path.DirectorySeparatorChar + GetValidFileName(fileName);
        }

        public static string GetValidURL(string url, bool replaceSpace = false)
        {
            if (replaceSpace) url = url.Replace(' ', '_');
            return HttpUtility.UrlPathEncode(url);
        }

        public static string GetAbsolutePath(string path)
        {
            path = ExpandFolderVariables(path);

            if (!Path.IsPathRooted(path)) // Is relative path?
            {
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            return Path.GetFullPath(path);
        }

        public static string GetTempPath(string extension)
        {
            string path = Path.GetTempFileName();
            return Path.ChangeExtension(path, extension);
        }
    }
}