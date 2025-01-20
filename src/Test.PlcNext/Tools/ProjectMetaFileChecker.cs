#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using PlcNext.Common.Project;
using Shouldly;

namespace Test.PlcNext.Tools
{
    internal class ProjectMetaFileChecker
    {
        private readonly ProjectSettings settings;
        private readonly string message;

        private ProjectMetaFileChecker(Stream fileStream)
        {
            message = string.Empty;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProjectSettings));
                settings = (ProjectSettings) serializer.Deserialize(fileStream);
            }
            catch (Exception e)
            {
                message = e.ToString();
                settings = null;
            }
        }

        public static ProjectMetaFileChecker Check(Stream fileStream)
        {
            return new ProjectMetaFileChecker(fileStream);
        }

        public ProjectMetaFileChecker SupportsTargetTypes(string[] targets, bool sorted)
        {
            settings.ShouldNotBeNull($"project settings could not be loaded. {message}");
            string[] actualTargets = settings.Target ?? new string[0];
            actualTargets.Length.ShouldBe(targets.Count());
            actualTargets.Except(targets).Any().ShouldBeFalse();
            targets.Except(actualTargets).Any().ShouldBeFalse();

            if (sorted)
            {
                actualTargets.SequenceEqual(targets).ShouldBeTrue();
            }

            return this;
        }
    }
}
