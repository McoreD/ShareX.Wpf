﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLib
{
    public static class StreamExtensions
    {
        private const int DefaultBufferSize = 4096;

        public static void CopyStreamTo(this Stream fromStream, Stream toStream, int bufferSize = DefaultBufferSize)
        {
            if (fromStream.CanSeek)
            {
                fromStream.Position = 0;
            }

            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = fromStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                toStream.Write(buffer, 0, bytesRead);
            }
        }

        public static int CopyStreamTo(this Stream fromStream, Stream toStream, int offset, int length, int bufferSize = DefaultBufferSize)
        {
            fromStream.Position = offset;

            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            int totalBytesRead = 0;
            int positionLimit = length - bufferSize;
            int readLength = bufferSize;

            do
            {
                if (totalBytesRead > positionLimit)
                {
                    readLength = length - totalBytesRead;
                }

                bytesRead = fromStream.Read(buffer, 0, readLength);
                toStream.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
            while (bytesRead > 0 && totalBytesRead < length);

            return totalBytesRead;
        }

        public static int CopyStreamTo64(this FileStream fromStream, Stream toStream, long offset, int length, int bufferSize = DefaultBufferSize)
        {
            fromStream.Position = offset;

            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            int totalBytesRead = 0;
            int positionLimit = length - bufferSize;
            int readLength = bufferSize;

            do
            {
                if (totalBytesRead > positionLimit)
                {
                    readLength = length - totalBytesRead;
                }

                bytesRead = fromStream.Read(buffer, 0, readLength);
                toStream.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
            while (bytesRead > 0 && totalBytesRead < length);

            return totalBytesRead;
        }

        public static bool WriteToFile(this Stream stream, string filePath)
        {
            if (stream.Length > 0 && !string.IsNullOrEmpty(filePath))
            {
                Helper.CreateDirectoryFromFilePath(filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    stream.CopyStreamTo(fileStream);
                }

                return true;
            }

            return false;
        }

        public static byte[] GetBytes(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyStreamTo(ms);
                return ms.ToArray();
            }
        }

        public static byte[] GetBytes(this Stream stream, int offset, int length)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyStreamTo(ms, offset, length);
                return ms.ToArray();
            }
        }

        public static T Read<T>(this Stream stream)
        {
            byte[] buffer = new byte[Marshal.SizeOf(typeof(T))];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes == 0) throw new InvalidOperationException("End-of-file reached");
            if (bytes != buffer.Length) throw new ArgumentException("File contains bad data");
            T retval;
            GCHandle hdl = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            try
            {
                retval = (T)Marshal.PtrToStructure(hdl.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                hdl.Free();
            }

            return retval;
        }

        public static void Write(this FileStream stream, byte[] array)
        {
            stream.Write(array, 0, array.Length);
        }
    }
}