#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace HelpersLib
{
    public class ReplCodeMenuEntry : CodeMenuEntry
    {
        public ReplCodeMenuEntry(string value, string description, string category = default(string))
            : base(value, description, category)
        {
        }

        public override String ToPrefixString()
        {
            return '%' + _value;
        }

        public static readonly ReplCodeMenuEntry t = new ReplCodeMenuEntry("t", "Title of active window", "Target");
        public static readonly ReplCodeMenuEntry pn = new ReplCodeMenuEntry("pn", "Process name of active window", "Target");
        public static readonly ReplCodeMenuEntry y = new ReplCodeMenuEntry("y", "Current year", "Date and time");
        public static readonly ReplCodeMenuEntry yy = new ReplCodeMenuEntry("yy", "Current year (2 digits)", "Date and time");
        public static readonly ReplCodeMenuEntry mo = new ReplCodeMenuEntry("mo", "Current month", "Date and time");
        public static readonly ReplCodeMenuEntry mon = new ReplCodeMenuEntry("mon", "Current month name (local language)", "Date and time");
        public static readonly ReplCodeMenuEntry mon2 = new ReplCodeMenuEntry("mon2", "Current month name (English)", "Date and time");
        public static readonly ReplCodeMenuEntry d = new ReplCodeMenuEntry("d", "Current day", "Date and time");
        public static readonly ReplCodeMenuEntry h = new ReplCodeMenuEntry("h", "Current hour", "Date and time");
        public static readonly ReplCodeMenuEntry mi = new ReplCodeMenuEntry("mi", "Current minute", "Date and time");
        public static readonly ReplCodeMenuEntry s = new ReplCodeMenuEntry("s", "Current day", "Date and time");
        public static readonly ReplCodeMenuEntry ms = new ReplCodeMenuEntry("ms", "Current millisecond", "Date and time");
        public static readonly ReplCodeMenuEntry pm = new ReplCodeMenuEntry("pm", "Gets AM/PM", "Date and time");
        public static readonly ReplCodeMenuEntry w = new ReplCodeMenuEntry("w", "Current week name (local language)", "Date and time");
        public static readonly ReplCodeMenuEntry w2 = new ReplCodeMenuEntry("w2", "Current week name (English)", "Date and time");
        public static readonly ReplCodeMenuEntry unix = new ReplCodeMenuEntry("unix", "UNIX timestamp", "Date and time");
        public static readonly ReplCodeMenuEntry i = new ReplCodeMenuEntry("i", "Auto increment number. 0 pad left using {n}", "Incremental");
        public static readonly ReplCodeMenuEntry ia = new ReplCodeMenuEntry("ia", "Auto increment alphanumeric case-insensitive. 0 pad left using {n}", "Incremental");
        public static readonly ReplCodeMenuEntry iAa = new ReplCodeMenuEntry("iAa", "Auto increment alphanumeric case-sensitive. 0 pad left using {n}", "Incremental");
        public static readonly ReplCodeMenuEntry ib = new ReplCodeMenuEntry("ib", "Auto increment by base {n} using alphanumeric (1 < n < 63)", "Incremental");
        public static readonly ReplCodeMenuEntry ix = new ReplCodeMenuEntry("ix", "Auto increment hexadecimal. 0 pad left using {n}", "Incremental");
        public static readonly ReplCodeMenuEntry rn = new ReplCodeMenuEntry("rn", "Random number 0 to 9. Repeat using {n}", "Random");
        public static readonly ReplCodeMenuEntry ra = new ReplCodeMenuEntry("ra", "Random alphanumeric char. Repeat using {n}", "Random");
        public static readonly ReplCodeMenuEntry rx = new ReplCodeMenuEntry("rx", "Random hexadecimal char. Repeat using {n}", "Random");
        public static readonly ReplCodeMenuEntry guid = new ReplCodeMenuEntry("guid", "Random GUID", "Random");
        public static readonly ReplCodeMenuEntry width = new ReplCodeMenuEntry("width", "Image width", "Image");
        public static readonly ReplCodeMenuEntry height = new ReplCodeMenuEntry("height", "Image height", "Image");
        public static readonly ReplCodeMenuEntry un = new ReplCodeMenuEntry("un", "User name", "Computer");
        public static readonly ReplCodeMenuEntry uln = new ReplCodeMenuEntry("uln", "Login name", "Computer");
        public static readonly ReplCodeMenuEntry cn = new ReplCodeMenuEntry("cn", "Computer name", "Computer");
        public static readonly ReplCodeMenuEntry n = new ReplCodeMenuEntry("n", "New line");
    }

    public enum NameParserType
    {
        Text, // Allows new line
        FileName,
        FolderPath,
        FilePath,
        URL
    }

    public class NameParser
    {
        public NameParserType Type { get; private set; }
        public int MaxNameLength { get; set; }
        public int MaxTitleLength { get; set; }
        public int AutoIncrementNumber { get; set; } // %i, %ia, %ib, %iAa, %ix
        public Image Picture { get; set; } // %width, %height
        public string WindowText { get; set; } // %t
        public string ProcessName { get; set; } // %pn
        public TimeZoneInfo CustomTimeZone { get; set; }

        protected NameParser()
        {
        }

        public NameParser(NameParserType nameParserType)
        {
            Type = nameParserType;
        }

        public static string Parse(NameParserType nameParserType, string pattern)
        {
            return new NameParser(nameParserType).Parse(pattern);
        }

        public string Parse(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(pattern);

            if (WindowText != null)
            {
                string windowText = WindowText.Replace(' ', '_');
                if (MaxTitleLength > 0 && windowText.Length > MaxTitleLength)
                {
                    windowText = windowText.Remove(MaxTitleLength);
                }
                sb.Replace(ReplCodeMenuEntry.t.ToPrefixString(), windowText);
            }

            if (ProcessName != null)
            {
                sb.Replace(ReplCodeMenuEntry.pn.ToPrefixString(), ProcessName);
            }

            string width = string.Empty, height = string.Empty;

            if (Picture != null)
            {
                width = Picture.Width.ToString();
                height = Picture.Height.ToString();
            }

            sb.Replace(ReplCodeMenuEntry.width.ToPrefixString(), width);
            sb.Replace(ReplCodeMenuEntry.height.ToPrefixString(), height);

            DateTime dt = DateTime.Now;

            if (CustomTimeZone != null)
            {
                dt = TimeZoneInfo.ConvertTime(dt, CustomTimeZone);
            }

            sb.Replace(ReplCodeMenuEntry.mon2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplCodeMenuEntry.mon.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month))
                .Replace(ReplCodeMenuEntry.yy.ToPrefixString(), dt.ToString("yy"))
                .Replace(ReplCodeMenuEntry.y.ToPrefixString(), dt.Year.ToString())
                .Replace(ReplCodeMenuEntry.mo.ToPrefixString(), Helper.AddZeroes(dt.Month))
                .Replace(ReplCodeMenuEntry.d.ToPrefixString(), Helper.AddZeroes(dt.Day));

            string hour;

            if (sb.ToString().Contains(ReplCodeMenuEntry.pm.ToPrefixString()))
            {
                hour = Helper.HourTo12(dt.Hour);
            }
            else
            {
                hour = Helper.AddZeroes(dt.Hour);
            }

            sb.Replace(ReplCodeMenuEntry.h.ToPrefixString(), hour)
                .Replace(ReplCodeMenuEntry.mi.ToPrefixString(), Helper.AddZeroes(dt.Minute))
                .Replace(ReplCodeMenuEntry.s.ToPrefixString(), Helper.AddZeroes(dt.Second))
                .Replace(ReplCodeMenuEntry.ms.ToPrefixString(), Helper.AddZeroes(dt.Millisecond, 3))
                .Replace(ReplCodeMenuEntry.w2.ToPrefixString(), CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplCodeMenuEntry.w.ToPrefixString(), CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(dt.DayOfWeek))
                .Replace(ReplCodeMenuEntry.pm.ToPrefixString(), dt.Hour >= 12 ? "PM" : "AM");

            sb.Replace(ReplCodeMenuEntry.unix.ToPrefixString(), DateTime.UtcNow.ToUnix().ToString());

            if (sb.ToString().Contains(ReplCodeMenuEntry.i.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ib.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ib.ToPrefixString().Replace('b', 'B'))
                || sb.ToString().Contains(ReplCodeMenuEntry.iAa.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA"))
                || sb.ToString().Contains(ReplCodeMenuEntry.ia.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A'))
                || sb.ToString().Contains(ReplCodeMenuEntry.ix.ToPrefixString())
                || sb.ToString().Contains(ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X')))
            {
                AutoIncrementNumber++;

                // Base
                try
                {
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), ReplCodeMenuEntry.ib.ToPrefixString(), 2))
                    {
                        sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helper.AlphanumericInverse), entry.Item2[1]));
                    }
                    foreach (Tuple<string, int[]> entry in ListEntryWithValues(sb.ToString(), ReplCodeMenuEntry.ib.ToPrefixString().Replace('b', 'B'), 2))
                    {
                        sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(entry.Item2[0], Helper.Alphanumeric), entry.Item2[1]));
                    }
                }
                catch
                {
                }

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.iAa.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(62, Helper.Alphanumeric), entry.Item2));
                }
                sb.Replace(ReplCodeMenuEntry.iAa.ToPrefixString(), AutoIncrementNumber.ToBase(62, Helper.Alphanumeric));

                // Alphanumeric Dual Case (Base 62)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA")))
                {
                    sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(62, Helper.AlphanumericInverse), entry.Item2));
                }
                sb.Replace(ReplCodeMenuEntry.iAa.ToPrefixString().Replace("Aa", "aA"), AutoIncrementNumber.ToBase(62, Helper.AlphanumericInverse));

                // Alphanumeric Single Case (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ia.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(36, Helper.Alphanumeric), entry.Item2).ToLowerInvariant());
                }
                sb.Replace(ReplCodeMenuEntry.ia.ToPrefixString(), AutoIncrementNumber.ToBase(36, Helper.Alphanumeric).ToLowerInvariant());

                // Alphanumeric Single Case Capital (Base 36)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A')))
                {
                    sb.Replace(entry.Item1, Helper.AddZeroes(AutoIncrementNumber.ToBase(36, Helper.Alphanumeric), entry.Item2).ToUpperInvariant());
                }
                sb.Replace(ReplCodeMenuEntry.ia.ToPrefixString().Replace('a', 'A'), AutoIncrementNumber.ToBase(36, Helper.Alphanumeric).ToUpperInvariant());

                // Hexadecimal (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ix.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("x" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.ix.ToPrefixString(), AutoIncrementNumber.ToString("x"));

                // Hexadecimal Capital (Base 16)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X')))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("X" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.ix.ToPrefixString().Replace('x', 'X'), AutoIncrementNumber.ToString("X"));

                // Number (Base 10)
                foreach (Tuple<string, int> entry in ListEntryWithValue(sb.ToString(), ReplCodeMenuEntry.i.ToPrefixString()))
                {
                    sb.Replace(entry.Item1, AutoIncrementNumber.ToString("d" + entry.Item2.ToString()));
                }
                sb.Replace(ReplCodeMenuEntry.i.ToPrefixString(), AutoIncrementNumber.ToString("d"));
            }

            sb.Replace(ReplCodeMenuEntry.un.ToPrefixString(), Environment.UserName);
            sb.Replace(ReplCodeMenuEntry.uln.ToPrefixString(), Environment.UserDomainName);
            sb.Replace(ReplCodeMenuEntry.cn.ToPrefixString(), Environment.MachineName);

            if (Type == NameParserType.Text)
            {
                sb.Replace(ReplCodeMenuEntry.n.ToPrefixString(), Environment.NewLine);
            }

            string result = sb.ToString();

            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rn.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helper.RepeatGenerator(entry.Item2, () => Helper.GetRandomChar(Helper.Numbers).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.ra.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helper.RepeatGenerator(entry.Item2, () => Helper.GetRandomChar(Helper.Alphanumeric).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rx.ToPrefixString()))
            {
                result = result.ReplaceAll(entry.Item1, () => Helper.RepeatGenerator(entry.Item2, () => Helper.GetRandomChar(Helper.Hexadecimal.ToLowerInvariant()).ToString()));
            }
            foreach (Tuple<string, int> entry in ListEntryWithValue(result, ReplCodeMenuEntry.rx.ToPrefixString().Replace('x', 'X')))
            {
                result = result.ReplaceAll(entry.Item1, () => Helper.RepeatGenerator(entry.Item2, () => Helper.GetRandomChar(Helper.Hexadecimal.ToUpperInvariant()).ToString()));
            }

            result = result.ReplaceAll(ReplCodeMenuEntry.rn.ToPrefixString(), () => Helper.GetRandomChar(Helper.Numbers).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.ra.ToPrefixString(), () => Helper.GetRandomChar(Helper.Alphanumeric).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.rx.ToPrefixString(), () => Helper.GetRandomChar(Helper.Hexadecimal.ToLowerInvariant()).ToString());
            result = result.ReplaceAll(ReplCodeMenuEntry.rx.ToPrefixString().Replace('x', 'X'), () => Helper.GetRandomChar(Helper.Hexadecimal.ToUpperInvariant()).ToString());

            result = result.ReplaceAll(ReplCodeMenuEntry.guid.ToPrefixString().ToLowerInvariant(), () => Guid.NewGuid().ToString().ToLowerInvariant());
            result = result.ReplaceAll(ReplCodeMenuEntry.guid.ToPrefixString().ToUpperInvariant(), () => Guid.NewGuid().ToString().ToUpperInvariant());

            if (Type == NameParserType.FolderPath)
            {
                result = Helper.GetValidFolderPath(result);
            }
            else if (Type == NameParserType.FileName)
            {
                result = Helper.GetValidFileName(result);
            }
            else if (Type == NameParserType.FilePath)
            {
                result = Helper.GetValidFilePath(result);
            }
            else if (Type == NameParserType.URL)
            {
                result = Helper.GetValidURL(result);
            }

            if (MaxNameLength > 0 && result.Length > MaxNameLength)
            {
                result = result.Remove(MaxNameLength);
            }

            return result;
        }

        private IEnumerable<Tuple<string, string[]>> ListEntryWithArguments(string text, string entry, int elements)
        {
            foreach (Tuple<string, string> o in text.ForEachBetween(entry + "{", "}"))
            {
                string[] s = o.Item2.Split(',');
                if (elements > s.Length)
                {
                    Array.Resize(ref s, elements);
                }
                yield return new Tuple<string, string[]>(o.Item1, s);
            }
        }

        private IEnumerable<Tuple<string, int[]>> ListEntryWithValues(string text, string entry, int elements)
        {
            foreach (Tuple<string, string[]> o in ListEntryWithArguments(text, entry, elements))
            {
                int[] a = new int[o.Item2.Length];
                for (int i = o.Item2.Length - 1; i >= 0; --i)
                {
                    int n = 0;
                    if (int.TryParse(o.Item2[i], out n))
                    {
                        a[i] = n;
                    }
                }
                yield return new Tuple<string, int[]>(o.Item1, a);
            }
        }

        private IEnumerable<Tuple<string, int>> ListEntryWithValue(string text, string entry)
        {
            foreach (Tuple<string, int[]> o in ListEntryWithValues(text, entry, 1))
            {
                yield return new Tuple<string, int>(o.Item1, o.Item2[0]);
            }
        }
    }
}