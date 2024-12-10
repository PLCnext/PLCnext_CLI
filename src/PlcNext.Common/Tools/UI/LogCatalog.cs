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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlcNext.Common.Tools.UI
{
    public static class LogCatalog
    {
        public static ILog CreateNewLog(string logCatalogPath, string executedCommand)
        {
            using (Mutex interProcessMutex = new Mutex(false, "Global\\ebce3df1-946c-4ec4-8369-cec004a40392"))
            {
                string filePath = null;
                if (interProcessMutex.WaitOne(1000))
                {
                    try
                    {
                        string directory = Path.GetDirectoryName(logCatalogPath) ?? string.Empty;
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        string logFilesDirectory = Path.Combine(directory, "LogFiles");
                        if (!Directory.Exists(logFilesDirectory))
                        {
                            Directory.CreateDirectory(logFilesDirectory);
                        }

                        if (!File.Exists(logCatalogPath))
                        {
                            File.WriteAllText(logCatalogPath, "{\"MaxCatalogSize\": 100}", Encoding.UTF8);
                        }

                        using (FileStream catalogStream = File.Open(logCatalogPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            JObject catalogContent = ReadCatalog(catalogStream);

                            int catalogSize = catalogContent.ContainsKey("MaxCatalogSize") &&
                                              catalogContent["MaxCatalogSize"].Type == JTokenType.Integer
                                                  ? catalogContent["MaxCatalogSize"].Value<int>()
                                                  : 100;
                            JArray files = catalogContent.ContainsKey("LogFiles") &&
                                           catalogContent["LogFiles"] is JArray filesArray
                                               ? filesArray
                                               : CreateLogFilesArray(catalogContent);

                            RemoveOldFiles(files, catalogSize, logFilesDirectory);

                            filePath = CreateFile(logFilesDirectory, files);

                            SaveCatalog(catalogStream, catalogContent);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while accessing log file catalog:{Environment.NewLine}{e}");
                    }
                    finally
                    {
                        interProcessMutex.ReleaseMutex();
                    }
                }
                else
                {
                    Console.WriteLine("Timeout while waiting for other processes to free log file catalog.");
                }
                return FileLog.Create(filePath);
            }

            JArray CreateLogFilesArray(JObject catalogContent)
            {
                catalogContent.Remove("LogFiles");
                
                JArray array = new JArray();
                catalogContent.Add("LogFiles", array);
                return array;
            }

            void RemoveOldFiles(JArray files, int catalogSize, string logFilesDirectory)
            {
                RemoveFilesOlderThanSixMonthsFromCatalog();

                int removableFiles = Math.Min(Math.Max(files.Count + 1 - catalogSize, 0), files.Count);
                List<string> filesToDelete = new List<string>();
                for (int i = 0; i < removableFiles; i++)
                {
                    JToken file = files[files.Count-1];
                    files.RemoveAt(files.Count-1);
                    if (file is JObject fileObject &&
                        fileObject.ContainsKey("File") &&
                        fileObject["File"].Type == JTokenType.String)
                    {
                        filesToDelete.Add(fileObject["File"].Value<string>());
                    }
                }

                List<string> knownFiles = new List<string>();
                foreach (JObject file in files.OfType<JObject>())
                {
                    if (file.ContainsKey("File") &&
                        file["File"].Type == JTokenType.String)
                    {
                        knownFiles.Add(file["File"].Value<string>());
                    }
                }

                foreach (string file in Directory.GetFiles(logFilesDirectory))
                {
                    if (!knownFiles.Contains(file))
                    {
                        SafeDeleteFile(file);
                    }
                }

                foreach (string file in filesToDelete)
                {
                    SafeDeleteFile(file);
                }

                void RemoveFilesOlderThanSixMonthsFromCatalog()
                {
                    for(int i=files.Count - 1; i >= 0; i--)
                    {
                        JToken fileToken = files[i];
                        if (fileToken is JObject file)
                        {
                            if (file.ContainsKey("Date") &&
                            file["Date"].Type == JTokenType.Date)
                            {
                                DateTime fileDate = file["Date"].Value<DateTime>();
                                if (fileDate.CompareTo(DateTime.Today.AddMonths(-6)) <= 0)
                                {
                                    files.RemoveAt(i);
                                }
                            }
                        }
                    }
                }

                void SafeDeleteFile(string file)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException)
                    {
                        //Nothing to do - will be deleted the next time
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //Nothing to do - will be deleted the next time
                    }
                }
            }

            JObject ReadCatalog(FileStream catalogStream)
            {
                JObject catalogContent;
                try
                {
                    using (StreamReader reader = new StreamReader(catalogStream, Encoding.UTF8, true, 4096, true))
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        catalogContent = JObject.Load(jsonReader);
                    }
                }
                catch (JsonReaderException)
                {
                    catalogContent = new JObject();
                }

                return catalogContent;
            }

            void SaveCatalog(FileStream catalogStream, JObject catalogContent)
            {
                catalogStream.SetLength(0);
                using (StreamWriter writer = new StreamWriter(catalogStream, Encoding.UTF8))
                using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    catalogContent.WriteTo(jsonWriter);
                }
            }

            string CreateFile(string logFilesDirectory, JArray files)
            {
                string filePath;
                do
                {
                    filePath = Path.Combine(logFilesDirectory, $"Log-{Guid.NewGuid().ToByteString()}.txt");
                } while (File.Exists(filePath));


                JObject newFileObject = new JObject(new JProperty("Date", new JValue(DateTime.Now)),
                                                    new JProperty("Command", new JValue(executedCommand)),
                                                    new JProperty("File", new JValue(filePath)));
                files.Insert(0, newFileObject);
                return filePath;
            }
        }
    }
}
