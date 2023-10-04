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
using System.Xml.Linq;
using System.Xml.Serialization;
using FluentAssertions;
using PlcNext.Common.Project;

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
            settings.Should().NotBeNull($"project settings could not be loaded. {message}");
            string[] actualTargets = settings.Target ?? new string[0];
            actualTargets.Length.Should().Be(targets.Count());
            actualTargets.Except(targets).Any().Should().BeFalse();
            targets.Except(actualTargets).Any().Should().BeFalse();

            if (sorted)
            {
                actualTargets.SequenceEqual(targets).Should().BeTrue();
            }

            return this;
        }
    }
}
