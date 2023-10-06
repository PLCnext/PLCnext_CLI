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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlcNext.Migration
{
    internal sealed class MigrationChain
    {
        private readonly Action<string> writeMessage;
        private readonly List<IConversionStep> steps = new List<IConversionStep>();
        private readonly List<string> files = new List<string>();
        private readonly List<(string Path, Version BaseVersion)> knownLocations = new List<(string, Version)>();
        private string migrationDestination = string.Empty;

        private MigrationChain(Action<string> writeMessage)
        {
            this.writeMessage = writeMessage;
        }

        public static MigrationChain Start(Action<string> writeMessage)
        {
            return new MigrationChain(writeMessage);
        }

        public MigrationChain AddConversionStep<T>()
            where T : IConversionStep, new()
        {
            steps.Add(new T());
            return this;
        }

        public MigrationChain AddPotentialLocation(string path, Version baseVersion)
        {
            knownLocations.Add((path, baseVersion));
            return this;
        }

        public MigrationChain AddMigrationFile(string fileName)
        {
            files.Add(fileName);
            return this;
        }

        public MigrationChain SetMigrationDestination(string path)
        {
            migrationDestination = path;
            return this;
        }

        public bool Execute()
        {
            writeMessage("Start migrating from old CLI version.");
            try
            {
                writeMessage($"1/?: Locate and copy old settings and caches.");
                if (TryLocateAndCopyOldFiles(out Version baseVersion))
                {
                    IConversionStep[] conversionSteps = steps.OrderBy(s => s.BaseVersion)
                                                             .SkipWhile(s => s.BaseVersion < baseVersion)
                                                             .ToArray();
                    Version currentVersion = GetCurrentVersion();
                    for (int i = 0; i < conversionSteps.Length; i++)
                    {
                        writeMessage($"{i + 2}/{conversionSteps.Length + 1}: Convert settings and caches " +
                                     $"from {conversionSteps[i].BaseVersion} " +
                                     $"to {(i < conversionSteps.Length - 2 ? conversionSteps[i + 1].BaseVersion.ToString() : currentVersion.ToString(3))}.");
                        conversionSteps[i].Execute(migrationDestination);
                    }
                }
            }
            catch (Exception e)
            {
                writeMessage($"Migration was not successful. Error during migration:{Environment.NewLine}{e}");
                return false;
            }

            writeMessage("Migration finished successfully.");
            return true;

            Version GetCurrentVersion()
            {
                Assembly executingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                return executingAssembly.GetName().Version;
            }

            bool TryLocateAndCopyOldFiles(out Version version)
            {
                foreach ((string path, Version baseVersion) in knownLocations.OrderByDescending(l => l.BaseVersion))
                {
                    if (Directory.Exists(path))
                    {
                        foreach (string file in files)
                        {
                            if (File.Exists(Path.Combine(path, file)))
                            {
                                if (!Directory.Exists(migrationDestination))
                                {
                                    Directory.CreateDirectory(migrationDestination);
                                }

                                File.Copy(Path.Combine(path, file), Path.Combine(migrationDestination, file), true);
                            }
                        }
                        version = baseVersion;
                        return true;
                    }
                }

                version = null;
                return false;
            }
        }
    }
}
