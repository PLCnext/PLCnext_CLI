#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PlcNext.Common.Tools.UI
{
    public class FileLog : ILog, IDisposable
    {
        private readonly Stream traceStream;
        private readonly TextWriterTraceListener textListener;

        private FileLog(Stream traceStream)
        {
            this.traceStream = traceStream;
            if (traceStream != null)
            {
                traceStream.SetLength(0);
                textListener = new TextWriterTraceListener(traceStream);
                Trace.Listeners.Add(textListener);
                Trace.AutoFlush = true;
            }
        }

        public static FileLog Create(string filePath)
        {
            Stream stream = null;
            Exception exception = null;
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    stream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            }

            FileLog log = new FileLog(stream);
            if (exception != null)
            {
                Console.WriteLine($"Exception while opening log file {filePath}:{Environment.NewLine}{exception}");
            }
            return log;
        }

        public void LogVerbose(string message)
        {
            Trace.TraceInformation($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: VERBOSE: {message}");
        }

        public void LogInformation(string message)
        {
            Trace.TraceInformation($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: INFORMATION: {message}");
        }

        public void LogWarning(string message)
        {
            Trace.TraceWarning($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: WARNING: {message}");
        }

        public void LogError(string message)
        {
            Trace.TraceError($"{DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}: ERROR: {message}");
        }

        public void Dispose()
        {
            Trace.Flush();
            Trace.Listeners.Remove(textListener);
            textListener?.Dispose();
            traceStream?.Dispose();
        }
    }
}
