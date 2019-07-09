#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlcNext.Common.Project.Persistence
{
    internal class CMakeFileGenerator
    {
        public static void WriteCMakeFile(VirtualFile cMakeFile, string name,
            string sourceDirectory = Constants.SourceFolderName)
        {
            using (Stream templateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PlcNext.Common.Project.templates.ProjectTemplates.CMakeLists.txt"))
            using (Stream fileStream = cMakeFile.OpenWrite())
            using (StreamReader reader = new StreamReader(templateStream))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                fileStream.SetLength(0);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace("$(sourceDirectory)", sourceDirectory);
                    line = line.Replace("$(name)", name);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
