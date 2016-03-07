using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLib
{
    public class Logger
    {
        public delegate void MessageAddedEventHandler(string message);

        public event MessageAddedEventHandler MessageAdded;

        public bool Async { get; set; } = true;
        public bool DebugWrite { get; set; } = true;
        public bool StoreInMemory { get; set; } = true;
        public bool FileWrite { get; set; } = false;
        public string LogFilePath { get; private set; }

        private readonly object loggerLock = new object();
        private StringBuilder sbMessages = new StringBuilder();

        public Logger()
        {
        }

        public Logger(string logFilePath)
        {
            FileWrite = true;
            LogFilePath = logFilePath;
            Helper.CreateDirectoryFromFilePath(LogFilePath);
        }

        protected void OnMessageAdded(string message)
        {
            if (MessageAdded != null)
            {
                MessageAdded(message);
            }
        }

        public void WriteLine(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (Async)
                {
                    Task.Run(() => WriteLineInternal(message));
                }
                else
                {
                    WriteLineInternal(message);
                }
            }
        }

        private void WriteLineInternal(string message)
        {
            lock (loggerLock)
            {
                message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";

                if (DebugWrite)
                {
                    Debug.WriteLine(message);
                }

                if (StoreInMemory && sbMessages != null)
                {
                    sbMessages.AppendLine(message);
                }

                if (FileWrite && !string.IsNullOrEmpty(LogFilePath))
                {
                    try
                    {
                        File.AppendAllText(LogFilePath, message + Environment.NewLine, Encoding.UTF8);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }

                OnMessageAdded(message);
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        public void WriteException(string exception, string message = "Exception")
        {
            WriteLine("{0}:{1}{2}", message, Environment.NewLine, exception);
        }

        public void WriteException(Exception exception, string message = "Exception")
        {
            WriteException(exception.ToString(), message);
        }

        public void Clear()
        {
            lock (loggerLock)
            {
                if (sbMessages != null)
                {
                    sbMessages.Clear();
                }
            }
        }

        public override string ToString()
        {
            lock (loggerLock)
            {
                if (sbMessages != null && sbMessages.Length > 0)
                {
                    return sbMessages.ToString();
                }

                return null;
            }
        }
    }
}