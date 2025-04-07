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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.FileSystem
{
    public class VirtualDirectory : VirtualEntry
    {
        private readonly IDirectoryContentResolver contentResolver;
        private readonly StringComparison pathEquality;
        private readonly List<VirtualEntry> entries = new List<VirtualEntry>();
        private List<VirtualEntry> entriesWhenCleared = new List<VirtualEntry>();
        private bool resolved;
        private readonly object resolveLock = new object();

        public VirtualDirectory(string name, IDirectoryContentResolver contentResolver,
                                StringComparison pathEquality) : base(name, contentResolver)
        {
            this.contentResolver = contentResolver;
            this.pathEquality = pathEquality;
        }

        public IEnumerable<VirtualEntry> Entries
        {
            get
            {
                if (resolved)
                {
                    return entries;
                }

                lock (resolveLock)
                {
                    if (resolved)
                    {
                        return entries;
                    }

                    foreach (VirtualEntry entry in contentResolver.GetContent())
                    {
                        AddEntry(entry);
                    }
                    resolved = true;
                }

                return entries;
            }
        }

        public async Task CopyToAsync(VirtualDirectory destination)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            foreach (VirtualFile file in Entries.OfType<VirtualFile>())
            {
                VirtualFile destinationFile = destination.File(file.Name);
                using (Stream destinationStream = destinationFile.OpenWrite())
                using (Stream sourceStream = file.OpenRead())
                {
                    await sourceStream.CopyToAsync(destinationStream).ConfigureAwait(false);
                }
            }

            foreach (VirtualDirectory directory in Directories)
            {
                await directory.CopyToAsync(destination.Directory(directory.Name)).ConfigureAwait(false);
            }
        }

        private bool Cleared { get; set; }
        public IEnumerable<VirtualDirectory> Directories => Entries.OfType<VirtualDirectory>();

        internal void AddEntry(VirtualEntry entry)
        {
            entries.Add(entry);
            entry.SetParent(this);
        }

        internal void RemoveEntry(VirtualEntry entry)
        {
            entries.Remove(entry);
        }

        public bool FileExists(string filename)
        {
            VirtualFile result = Entries.OfType<VirtualFile>().FirstOrDefault(d => d.Name.Equals(filename, pathEquality));
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckDirectlyFileExists(string filename)
        {
            VirtualFile result = contentResolver.GetContent().OfType<VirtualFile>().FirstOrDefault(d => d.Name.Equals(filename, pathEquality));
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public bool DirectoryExists(string directoryName)
        {
            VirtualDirectory result = Entries.OfType<VirtualDirectory>().FirstOrDefault(d => d.Name.Equals(directoryName, pathEquality));
            if (result == null)
            {
                return false;
            }
            return true;
        }

        public VirtualDirectory Directory(params string[] directoryParts)
        {
            if (directoryParts == null)
            {
                throw new ArgumentNullException(nameof(directoryParts));
            }

            if (!directoryParts.Any())
            {
                return this;
            }

            string directoryName = directoryParts[0];
            VirtualDirectory result = Entries.OfType<VirtualDirectory>().FirstOrDefault(d => d.Name.Equals(directoryName, pathEquality));
            if (result == null)
            {
                result = contentResolver.Create<VirtualDirectory>(directoryName);
                AddEntry(result);
            }
            return result.Directory(directoryParts.Skip(1).ToArray());
        }

        public VirtualFile File(string fileName)
        {
            VirtualFile result = Entries.OfType<VirtualFile>().FirstOrDefault(d => d.Name.Equals(fileName, pathEquality));
            if (result == null)
            {
                result = contentResolver.Create<VirtualFile>(fileName);
                AddEntry(result);
            }
            return result;
        }

        public IEnumerable<VirtualFile> Files(string searchString = "*", bool searchRecursive = false)
        {
            if (searchString == null)
            {
                throw new ArgumentNullException(nameof(searchString));
            }

            string regexPattern = searchString
                .Replace(".", "\\.", StringComparison.Ordinal)
                .Replace("*", ".*", StringComparison.Ordinal)
                .Replace("?", ".", StringComparison.Ordinal);

            Regex pattern = new Regex(regexPattern);
            return Files(pattern, searchRecursive);
        }

        private IEnumerable<VirtualFile> Files(Regex pattern, bool searchRecursive)
        {
            foreach (VirtualFile file in Entries.OfType<VirtualFile>()
                                                .Where(f => pattern.IsMatch(f.Name)))
            {
                yield return file;
            }

            if (searchRecursive)
            {
                foreach (VirtualFile file in Entries.OfType<VirtualDirectory>().SelectMany(d => d.Files(pattern, true)))
                {
                    yield return file;
                }
            }
        }

        public override void UnDelete(bool checkDeleted = true)
        {
            base.UnDelete(checkDeleted);
            UnClear();
        }

        public override void Delete()
        {
            Clear();
            base.Delete();
        }
        
        public void DeleteIfEmpty()
        {
            if (!Entries.Any())
            {
                base.Delete();
            }
        }

        public void Clear()
        {
            entriesWhenCleared = Entries.ToList();
            foreach (VirtualEntry entry in entriesWhenCleared)
            {
                entry.Delete();
            }
            Cleared = true;
        }

        public void UnClear()
        {
            if (!Cleared)
            {
                throw new InvalidOperationException("Cannot unclear a non cleared directory.");
            }
            foreach (VirtualEntry entry in entriesWhenCleared)
            {
                if (entry.Deleted)
                {
                    entry.UnDelete();
                }
            }
            Cleared = false;
            entriesWhenCleared.Clear();
        }

        public bool TryGetFileFromPath(string path, out VirtualFile file)
        {
            path = path.CleanPath();
            string[] parts = contentResolver.SplitPath(path);
            VirtualDirectory current = this;
            foreach (string part in parts.Take(parts.Length -1))
            {
                if (part == ".")
                {
                    continue;
                }

                if (part == "..")
                {
                    current = current.Parent;
                }
                else
                {
                    current = current.Entries.OfType<VirtualDirectory>()
                                     .FirstOrDefault(d => d.Name == part);
                }

                if (current == null)
                {
                    file = null;
                    return false;
                }
            }

            file = current.Entries.OfType<VirtualFile>().FirstOrDefault(f => f.Name.Equals(parts[parts.Length - 1], pathEquality));
            return file != null;
        }
        
        public override string ToString()
        {
            return FullName; //(Parent != null ? $"{Parent}/" : "") + Name;
        }
    }
}
