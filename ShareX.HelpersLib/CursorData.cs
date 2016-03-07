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
using System.Runtime.InteropServices;
using System.Windows;

namespace HelpersLib
{
    public class CursorData : IDisposable
    {
        public bool IsVisible { get; private set; }
        public IntPtr IconHandle { get; private set; }
        public Point Position { get; private set; }

        public CursorData()
        {
            UpdateCursorData();
        }

        public void UpdateCursorData()
        {
            CursorInfo cursorInfo = new CursorInfo();
            cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

            if (NativeMethods.GetCursorInfo(out cursorInfo))
            {
                IsVisible = cursorInfo.flags == NativeMethods.CURSOR_SHOWING;

                if (IsVisible)
                {
                    IconHandle = NativeMethods.CopyIcon(cursorInfo.hCursor);
                    IconInfo iconInfo;

                    if (NativeMethods.GetIconInfo(IconHandle, out iconInfo))
                    {
                        Point cursorPosition = CaptureHelper.GetZeroBasedMousePosition();
                        Position = new Point(cursorPosition.X - iconInfo.xHotspot, cursorPosition.Y - iconInfo.yHotspot);

                        if (iconInfo.hbmMask != IntPtr.Zero)
                        {
                            NativeMethods.DeleteObject(iconInfo.hbmMask);
                        }

                        if (iconInfo.hbmColor != IntPtr.Zero)
                        {
                            NativeMethods.DeleteObject(iconInfo.hbmColor);
                        }
                    }
                }
            }
        }

        public void DrawCursorToImage(System.Drawing.Image img)
        {
            DrawCursorToImage(img, new Point());
        }

        public void DrawCursorToImage(System.Drawing.Image img, Point cursorOffset)
        {
            if (IconHandle != IntPtr.Zero)
            {
                Point drawPosition = new Point(Position.X - cursorOffset.X, Position.Y - cursorOffset.Y);

                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
                using (System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(IconHandle))
                {
                    g.DrawIcon(icon, (int)drawPosition.X, (int)drawPosition.Y);
                }
            }
        }

        public void DrawCursorToHandle(IntPtr hdcDest)
        {
            DrawCursorToHandle(hdcDest, new Point());
        }

        public void DrawCursorToHandle(IntPtr hdcDest, Point cursorOffset)
        {
            if (IconHandle != IntPtr.Zero)
            {
                Point drawPosition = new Point(Position.X - cursorOffset.X, Position.Y - cursorOffset.Y);
                NativeMethods.DrawIconEx(hdcDest, (int)drawPosition.X, (int)drawPosition.Y, IconHandle, 0, 0, 0, IntPtr.Zero, NativeMethods.DI_NORMAL);
            }
        }

        public void Dispose()
        {
            if (IconHandle != IntPtr.Zero)
            {
                NativeMethods.DestroyIcon(IconHandle);
                IconHandle = IntPtr.Zero;
            }
        }
    }
}