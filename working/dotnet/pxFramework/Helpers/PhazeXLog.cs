namespace PhazeX.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This class will log all data to the event log. Override if you want it to go a different source.
    /// </summary>
    public class PhazeXLog
    {
        private static readonly string newLine = "\r\n";
        private static bool writeFileLog = true;
        
        public delegate void InitLogDelegate();
        public delegate void LogErrorDelegate(Exception e, string source, int code);
        public delegate void LogWarningDelegate(string text, string source, int code);
        public delegate void LogInformationDelegate(string text, string source);
        public delegate void LogDebugDelegate(string text, string source);        
        
        public static event InitLogDelegate OnInitLog;
        public static event LogErrorDelegate OnLogError;
        public static event LogWarningDelegate OnLogWarning;
        public static event LogInformationDelegate OnLogInformation;
        public static event LogDebugDelegate OnLogDebug;

        public static bool WriteEventLog
        {
            get;
            set;
        }

        public static bool WriteFileLog
        {
            get
            {
                return writeFileLog;
            }

            set
            {
                writeFileLog = value;
            }
        }

        public static string Filename
        {
            get;
            set;
        }

        public static void InitLog()
        {
            if (PhazeXLog.writeFileLog)
            {
                try
                {
                    if (Filename != null)
                    {
                        StreamWriter sw = new StreamWriter(Filename, false, Encoding.ASCII);
                        sw.Write("PhazeXLog" + newLine);
                        sw.Write(DateTime.Now.ToString() + newLine);
                        sw.Close();
                    }
                }
                catch
                {
                }
            }

            if (OnInitLog != null)
            {
                OnInitLog.Invoke();
            }
        }

        [Conditional("DEBUG")]
        public static void LogDebug(string text)
        {
            LogDebug(text, "PhazeX Debug");
        }

        /// <summary>
        /// Logs some debug information to the file log only (if writeFileLog is off, nothing will happen).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="source"></param>
        [Conditional("DEBUG")]
        public static void LogDebug(string text, string source)
        {
            if (PhazeXLog.writeFileLog)
            {
                try
                {
                    if (Filename != null)
                    {
                        StreamWriter sw = new StreamWriter(Filename, true, Encoding.ASCII);
                        DateTime dt = DateTime.Now;
                        sw.Write("[---debug---] [" + dt.ToShortDateString() + " " + dt.ToLongTimeString() + "] ");
                        sw.Write(text);
                        sw.Write(" (" + source + ")");
                        sw.Write(newLine);
                        sw.Close();
                    }
                }
                catch
                {
                }
            }

            if (OnLogDebug != null)
            {
                OnLogDebug.Invoke(text, source);
            }
        }

        public static void LogInformation(string text)
        {
            LogInformation(text, "PhazeX");
        }

        /// <summary>
        /// Log an information event to the event log. If we can't write to the event log, nothing happens.
        /// </summary>
        /// <param name="text">The event text.</param>
        /// <param name="pid">The event program source.</param>
        public static void LogInformation(string text, string pid)
        {
            if (PhazeXLog.writeFileLog)
            {
                try
                {
                    if (Filename != null)
                    {
                        StreamWriter sw = new StreamWriter(Filename, true, Encoding.ASCII);
                        DateTime dt = DateTime.Now;
                        sw.Write("[Information] [" + dt.ToShortDateString() + " " + dt.ToLongTimeString() + "] ");
                        sw.Write(text + newLine);
                        sw.Close();
                    }
                }
                catch
                {
                }
            }

#if !PocketPC
            if (WriteEventLog)
            {
                try
                {
                    EventLog.WriteEntry(pid, text, EventLogEntryType.Information);
                }
                catch
                {
                }
            }

#endif
            if (OnLogInformation != null)
            {
                OnLogInformation.Invoke(text, pid);
            }
        }

        /// <summary>
        /// Log a warning event to the event log. If we can't write to the event log, nothing happens.
        /// </summary>
        /// <param name="text">The event text</param>
        /// <param name="pid">The event program source.</param>
        /// <param name="code">The event code</param>
        public static void LogWarning(string text, string pid, int code)
        {
            if (PhazeXLog.writeFileLog)
            {
                try
                {
                    if (Filename != null)
                    {
                        StreamWriter sw = new StreamWriter(Filename, true, Encoding.ASCII);
                        DateTime dt = DateTime.Now;
                        sw.Write("[  Warning  ] [" + dt.ToShortDateString() + " " + dt.ToLongTimeString() + "] ");
                        sw.Write(text + newLine);
                        sw.Close();
                    }
                }
                catch
                {
                }
            }

#if !PocketPC
            if (WriteEventLog)
            {
                try
                {
                    EventLog.WriteEntry(pid, text, EventLogEntryType.Warning, code);
                }
                catch
                {
                    // don't do anything -- this was our last resort
                }
            }
#endif
            if (OnLogWarning != null)
            {
                OnLogWarning.Invoke(text, pid, code);
            }
        }

        /// <summary>
        /// Log a error event to the event log. If we can't write to the event log, nothing happens.
        /// </summary>
        /// <param name="text">The event text</param>
        /// <param name="pid">The event program source.</param>
        /// <param name="code">The event code</param>
        public static void LogError(Exception e, string pid, int code)
        {
            if (PhazeXLog.writeFileLog)
            {
                try
                {
                    if (Filename != null)
                    {
                        StreamWriter sw = new StreamWriter(Filename, true, Encoding.ASCII);
                        DateTime dt = DateTime.Now;
                        sw.Write("[   ERROR   ] [" + dt.ToShortDateString() + " " + dt.ToLongTimeString() + "] ");
                        sw.Write(e.Message + newLine);
                        sw.Write(e.StackTrace + newLine);
                        sw.Close();
                    }
                }
                catch
                {
                }
            }
#if !PocketPC
            if (WriteEventLog)
            {
                try
                {
                    EventLog.WriteEntry(pid, e.ToString(), EventLogEntryType.Error, code);
                }
                catch
                {
                    // don't do anything -- this was our last resort
                }
            }
#endif

            if (OnLogError != null)
            {
                OnLogError.Invoke(e, pid, code);
            }
        }
    }
}