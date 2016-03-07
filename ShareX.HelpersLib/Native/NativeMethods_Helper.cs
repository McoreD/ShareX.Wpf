using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HelpersLib
{
    public static partial class NativeMethods
    {
        public static string GetForegroundWindowText()
        {
            IntPtr handle = GetForegroundWindow();
            return GetWindowText(handle);
        }

        public static string GetWindowText(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                try
                {
                    int length = GetWindowTextLength(handle);

                    if (length > 0)
                    {
                        StringBuilder sb = new StringBuilder(length + 1);

                        if (GetWindowText(handle, sb, sb.Capacity) > 0)
                        {
                            return sb.ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        public static Process GetForegroundWindowProcess()
        {
            IntPtr handle = GetForegroundWindow();
            return GetProcessByWindowHandle(handle);
        }

        public static Process GetProcessByWindowHandle(IntPtr hwnd)
        {
            if (hwnd.ToInt32() > 0)
            {
                try
                {
                    uint processID;
                    GetWindowThreadProcessId(hwnd, out processID);
                    return Process.GetProcessById((int)processID);
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return null;
        }

        public static string GetClassName(IntPtr handle)
        {
            if (handle.ToInt32() > 0)
            {
                StringBuilder sb = new StringBuilder(256);

                if (GetClassName(handle, sb, sb.Capacity) > 0)
                {
                    return sb.ToString();
                }
            }

            return null;
        }

        public static IntPtr GetClassLongPtrSafe(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
            {
                return GetClassLongPtr(hWnd, nIndex);
            }

            return new IntPtr(GetClassLong(hWnd, nIndex));
        }

        public static bool GetBorderSize(IntPtr handle, out Size size)
        {
            WINDOWINFO wi = new WINDOWINFO();

            bool result = GetWindowInfo(handle, ref wi);

            if (result)
            {
                size = new Size((int)wi.cxWindowBorders, (int)wi.cyWindowBorders);
            }
            else
            {
                size = Size.Empty;
            }

            return result;
        }

        public static bool GetWindowRegion(IntPtr hWnd, out System.Drawing.Region region)
        {
            IntPtr hRgn = CreateRectRgn(0, 0, 0, 0);
            RegionType regionType = (RegionType)GetWindowRgn(hWnd, hRgn);
            region = System.Drawing.Region.FromHrgn(hRgn);
            return regionType != RegionType.ERROR && regionType != RegionType.NULLREGION;
        }

        public static bool IsDWMEnabled()
        {
            return Helper.IsWindowsVistaOrGreater() && DwmIsCompositionEnabled();
        }

        public static bool GetExtendedFrameBounds(IntPtr handle, out Rect Rect)
        {
            RECT rect;
            int result = DwmGetWindowAttribute(handle, (int)DwmWindowAttribute.ExtendedFrameBounds, out rect, Marshal.SizeOf(typeof(RECT)));
            Rect = rect;
            return result == 0;
        }

        public static bool GetNCRenderingEnabled(IntPtr handle)
        {
            bool enabled;
            int result = DwmGetWindowAttribute(handle, (int)DwmWindowAttribute.NCRenderingEnabled, out enabled, sizeof(bool));
            return result == 0 && enabled;
        }

        public static void SetNCRenderingPolicy(IntPtr handle, DwmNCRenderingPolicy renderingPolicy)
        {
            int renderPolicy = (int)renderingPolicy;
            DwmSetWindowAttribute(handle, (int)DwmWindowAttribute.NCRenderingPolicy, ref renderPolicy, sizeof(int));
        }

        public static Rect GetWindowRect(IntPtr handle)
        {
            RECT rect;
            GetWindowRect(handle, out rect);
            return rect;
        }

        public static Rect GetClientRect(IntPtr handle)
        {
            RECT rect;
            GetClientRect(handle, out rect);
            Point position = rect.Location;
            ClientToScreen(handle, ref position);
            return new Rect(position, rect.Size);
        }

        public static Rect MaximizedWindowFix(IntPtr handle, Rect windowRect)
        {
            Size size;

            if (GetBorderSize(handle, out size))
            {
                windowRect = new Rect(windowRect.X + size.Width, windowRect.Y + size.Height, windowRect.Width - (size.Width * 2), windowRect.Height - (size.Height * 2));
            }

            return windowRect;
        }

        public static void ActivateWindow(IntPtr handle)
        {
            SetForegroundWindow(handle);
            SetActiveWindow(handle);
        }

        public static Rect GetTaskbarRect()
        {
            APPBARDATA abd = APPBARDATA.NewAPPBARDATA();
            SHAppBarMessage((uint)ABMsg.ABM_GETTASKBARPOS, ref abd);
            return abd.rc;
        }

        public static bool SetTaskbarVisibilityIfIntersect(bool visible, Rect rect)
        {
            bool result = false;

            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (taskbarHandle != IntPtr.Zero)
            {
                Rect taskbarRect = GetWindowRect(taskbarHandle);

                if (rect.IntersectsWith(taskbarRect))
                {
                    ShowWindow(taskbarHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                    result = true;
                }

                if (Helper.IsWindowsVista() || Helper.IsWindows7())
                {
                    IntPtr startHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);

                    if (startHandle != IntPtr.Zero)
                    {
                        Rect startRect = GetWindowRect(startHandle);

                        if (rect.IntersectsWith(startRect))
                        {
                            ShowWindow(startHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                            result = true;
                        }
                    }
                }
            }

            return result;
        }

        public static bool SetTaskbarVisibility(bool visible)
        {
            IntPtr taskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (taskbarHandle != IntPtr.Zero)
            {
                ShowWindow(taskbarHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);

                if (Helper.IsWindowsVista() || Helper.IsWindows7())
                {
                    IntPtr startHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);

                    if (startHandle != IntPtr.Zero)
                    {
                        ShowWindow(startHandle, visible ? (int)WindowShowStyle.Show : (int)WindowShowStyle.Hide);
                    }
                }

                return true;
            }

            return false;
        }

        public static void TrimMemoryUse()
        {
            GC.Collect();
            GC.WaitForFullGCComplete();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (IntPtr)(-1), (IntPtr)(-1));
        }

        public static bool IsWindowMaximized(IntPtr handle)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            wp.length = Marshal.SizeOf(wp);
            GetWindowPlacement(handle, ref wp);
            return wp.showCmd == WindowShowStyle.Maximize;
        }

        public static IntPtr SetHook(int hookType, HookProc hookProc)
        {
            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(hookType, hookProc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }

        public static void RestoreWindow(IntPtr handle)
        {
            WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
            wp.length = Marshal.SizeOf(wp);
            GetWindowPlacement(handle, ref wp);

            if (wp.flags == (int)WindowPlacementFlags.WPF_RESTORETOMAXIMIZED)
            {
                wp.showCmd = WindowShowStyle.ShowMaximized;
            }
            else
            {
                wp.showCmd = WindowShowStyle.Restore;
            }

            SetWindowPlacement(handle, ref wp);
        }

        /// <summary>
        /// Version of <see cref="AVISaveOptions(IntPtr, int, int, IntPtr[], IntPtr[])"/> for one stream only.
        /// </summary>
        ///
        /// <param name="stream">Stream to configure.</param>
        /// <param name="options">Stream options.</param>
        ///
        /// <returns>Returns TRUE if the user pressed OK, FALSE for CANCEL, or an error otherwise.</returns>
        public static int AVISaveOptions(IntPtr stream, ref AVICOMPRESSOPTIONS options, IntPtr parentWindow)
        {
            IntPtr[] streams = new IntPtr[1];
            IntPtr[] infPtrs = new IntPtr[1];

            // alloc unmanaged memory
            IntPtr mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AVICOMPRESSOPTIONS)));

            // copy from managed structure to unmanaged memory
            Marshal.StructureToPtr(options, mem, false);

            streams[0] = stream;
            infPtrs[0] = mem;

            // show dialog with a list of available compresors and configuration
            int ret = AVISaveOptions(parentWindow, 0, 1, streams, infPtrs);

            // copy from unmanaged memory to managed structure
            options = (AVICOMPRESSOPTIONS)Marshal.PtrToStructure(mem, typeof(AVICOMPRESSOPTIONS));

            // free AVI compression options
            AVISaveOptionsFree(1, infPtrs);

            // clear it, because the information already freed by AVISaveOptionsFree
            options.format = 0;
            options.parameters = 0;

            // free unmanaged memory
            Marshal.FreeHGlobal(mem);

            return ret;
        }

        /// <summary>
        /// .NET replacement of mmioFOURCC macros. Converts four characters to code.
        /// </summary>
        ///
        /// <param name="str">Four characters string.</param>
        ///
        /// <returns>Returns the code created from provided characters.</returns>
        public static int mmioFOURCC(string str)
        {
            return (
                (byte)(str[0]) |
                ((byte)(str[1]) << 8) |
                ((byte)(str[2]) << 16) |
                ((byte)(str[3]) << 24));
        }

        /// <summary>
        /// Inverse to <see cref="mmioFOURCC"/>. Converts code to fout characters string.
        /// </summary>
        ///
        /// <param name="code">Code to convert.</param>
        ///
        /// <returns>Returns four characters string.</returns>
        public static string decode_mmioFOURCC(int code)
        {
            char[] chs = new char[4];

            for (int i = 0; i < 4; i++)
            {
                chs[i] = (char)(byte)((code >> (i << 3)) & 0xFF);
                if (!char.IsLetterOrDigit(chs[i]))
                    chs[i] = ' ';
            }
            return new string(chs);
        }

        public static bool Is64Bit()
        {
            return IntPtr.Size == 8 || (IntPtr.Size == 4 && Is32BitProcessOn64BitProcessor());
        }

        private static bool Is32BitProcessOn64BitProcessor()
        {
            bool retVal;
            IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
            return retVal;
        }
    }
}