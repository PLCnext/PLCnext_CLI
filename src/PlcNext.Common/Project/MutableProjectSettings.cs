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
using System.Text;
using System.Xml.Serialization;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.IO;

namespace PlcNext.Common.Project
{
    public class MutableProjectSettings
    {
        private readonly VirtualFile projectFile;
        private readonly ExecutionContext executionContext;
        public ProjectSettings Value { get; }

        internal MutableProjectSettings(ProjectSettings value, VirtualFile projectFile, ExecutionContext executionContext)
        {
            this.projectFile = projectFile;
            this.executionContext = executionContext;
            Value = value;
        }

        public MutableProjectSettings()
        {
            Value = new ProjectSettings();
        }

        public void AddTarget(string target)
        {
            AddTargets(new[] {target});
        }

        public void AddTargets(IEnumerable<string> targets)
        {
            SetTargets((Value.Target ?? new string[0]).Concat(targets));
        }

        public bool RemoveTarget(string target)
        {
            return RemoveTargets(new []{target}) == 1;
        }

        public int RemoveTargets(IEnumerable<string> targets)
        {
            IEnumerable<string> existingTargets = (Value.Target ?? new string[0]).Intersect(targets)
                                                                                 .ToArray();
            SetTargets((Value.Target ?? new string[0]).Except(existingTargets));
            return existingTargets.Count();
        }

        public void ClearTargets()
        {
            SetTargets(new string[0]);
        }

        public void SetTargets(IEnumerable<string> targets)
        {
            Value.Target = targets.ToArray();
            UpdateProjectSettingsFile("Update project targets.");
        }

        public void AddExtension(extension extension)
        {
            Value.Extension = Value.Extension.Concat(new[] {extension}).ToArray();
            UpdateProjectSettingsFile($"Add extension {extension.name}");
        }

        public void RemoveExtension(extension extension)
        {
            Value.Extension = Value.Extension.Except(new[] { extension }).ToArray();
            UpdateProjectSettingsFile($"Remove extension {extension.name}");
        }

        public void UpdateExtension(extension extension)
        {
            UpdateProjectSettingsFile($"Update extension {extension.name}");
        }

        public void SetId(string id)
        {
            Value.Id = id;
            UpdateProjectSettingsFile("Set project id.");
        }

        public bool IsPersistent => projectFile != null;

        private void UpdateProjectSettingsFile(string message)
        {
            if (projectFile == null)
            {
                //do not persist changes
                return;
            }
            MemoryStream revertStream = RecyclableMemoryStreamManager.Instance.GetStream();
            using (Stream fileStream = projectFile.OpenRead())
            {
                fileStream.CopyTo(revertStream);
                revertStream.Seek(0, SeekOrigin.Begin);
            }
            using (Stream fileStream = projectFile.OpenWrite())
            {
                //if an existing file content is longer than the new content, the xml is corrupted
                //set the stream legth to 0 to avoid this
                fileStream.SetLength(0);
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectSettings));
                serializer.Serialize(fileStream, Value);
            }
            executionContext.Observable.OnNext(new ProjectSettingChange(() =>
            {
                using (revertStream)
                using (Stream fileStream = projectFile.OpenWrite())
                {
                    //if an existing file content is longer than the new content, the xml is corrupted
                    //set the stream legth to 0 to avoid this
                    fileStream.SetLength(0);
                    revertStream.CopyTo(fileStream);
                }
            }, projectFile.Parent.FullName, message));
        }
    }
}
