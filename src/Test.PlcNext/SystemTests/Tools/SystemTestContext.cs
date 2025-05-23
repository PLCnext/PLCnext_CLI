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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using CSharpx;
using Mono.Cecil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using PlcNext.Common.CommandLine;
using PlcNext.Common.MetaData;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.UI;
using Shouldly;
using Test.PlcNext.SystemTests.Features;
using Test.PlcNext.Tools;
using Test.PlcNext.Tools.Abstractions;
using FieldDefinition = PlcNext.Common.MetaData.FieldDefinition;

namespace Test.PlcNext.SystemTests.Tools
{
    public class SystemTestContext : IDisposable
    {
        public enum SettingChange
        {
            Change,
            Add,
            Remove,
            Clear
        }

        public enum TargetChange
        {
            Add,
            Remove,
            None
        }

        private readonly IFileSystemAbstraction fileSystemAbstraction;
        private readonly IDownloadServiceAbstraction downloadServiceAbstraction;
        private readonly IProcessManagerAbstraction processManagerAbstraction;
        private readonly IEnvironmentServiceAbstraction environmentServiceAbstraction;
        private readonly IUserInterfaceAbstraction userInterfaceAbstraction;
        private readonly IExceptionHandlerAbstraction exceptionHandlerAbstraction;
        private readonly IGuidFactoryAbstraction guidFactoryAbstraction;
        private readonly ICMakeConversationAbstraction cmakeConversationAbstraction;
        private readonly ISdkExplorerAbstraction sdkExplorerAbstraction;
        private readonly IMSBuildFinderAbstraction msBuildFinderAbstraction;
        private readonly IPasswordProviderAbstraction passwordProviderAbstraction;
        private readonly bool autoActivatedComponents;
        private IContainer host;

        private const string MetadataNameSpace = "http://www.phoenixcontact.com/schema/metaconfig";
        private const string projectFileName = Constants.ProjectFileName;

        internal IContainer Host
        {
            get
            {
                if (!Initialized)
                {
                    throw new InvalidOperationException("Test context not initialized");
                }
                return host;
            }
            set => host = value;
        }

        internal void SetWorkspaceName(string workspaceName)
        {
            fileSystemAbstraction.CurrentDirectory.Name.Returns(workspaceName);
        }

        internal SystemTestContext(IFileSystemAbstraction fileSystemAbstraction, IDownloadServiceAbstraction downloadServiceAbstraction,
            IProcessManagerAbstraction processManagerAbstraction, IUserInterfaceAbstraction userInterfaceAbstraction,
            IEnvironmentServiceAbstraction environmentServiceAbstraction, IExceptionHandlerAbstraction exceptionHandlerAbstraction,
            IGuidFactoryAbstraction guidFactoryAbstraction, ICMakeConversationAbstraction cmakeConversationAbstraction,
            ISdkExplorerAbstraction sdkExplorerAbstraction, IMSBuildFinderAbstraction msBuildFinderAbstraction,
            IPasswordProviderAbstraction passwordProviderAbstraction, bool autoActivatedComponents = true)
        {
            this.fileSystemAbstraction = fileSystemAbstraction;
            this.downloadServiceAbstraction = downloadServiceAbstraction;
            this.processManagerAbstraction = processManagerAbstraction;
            this.userInterfaceAbstraction = userInterfaceAbstraction;
            this.environmentServiceAbstraction = environmentServiceAbstraction;
            this.exceptionHandlerAbstraction = exceptionHandlerAbstraction;
            this.guidFactoryAbstraction = guidFactoryAbstraction;
            this.cmakeConversationAbstraction = cmakeConversationAbstraction;
            this.sdkExplorerAbstraction = sdkExplorerAbstraction;
            this.msBuildFinderAbstraction = msBuildFinderAbstraction;
            this.passwordProviderAbstraction = passwordProviderAbstraction;
            this.autoActivatedComponents = autoActivatedComponents;
        }

        public bool Initialized { get; private set; }

        private ICommandLineParser commandLineParser;

        private ICommandLineParser CommandLineParser
        {
            get
            {
                if (commandLineParser == null)
                {
                    commandLineParser = new VerboseCommandLineParser(Host.Resolve<ICommandLineParser>(),
                                                                     Host.Resolve<IExceptionHandler>());
                }

                return commandLineParser;
            }
        }

        public void Initialize(Action<string> printMessage, Action<ContainerBuilder> buildAction = null)
        {
            InstancesRegistrationSource exportProvider = new InstancesRegistrationSource();
            fileSystemAbstraction.Initialize(exportProvider, printMessage);
            downloadServiceAbstraction.Initialize(exportProvider, printMessage);
            processManagerAbstraction?.Initialize(exportProvider, printMessage);
            userInterfaceAbstraction?.Initialize(exportProvider, printMessage);
            environmentServiceAbstraction.Initialize(exportProvider, printMessage);
            exceptionHandlerAbstraction.Initialize(exportProvider, printMessage);
            guidFactoryAbstraction.Initialize(exportProvider, printMessage);
            cmakeConversationAbstraction.Initialize(exportProvider, printMessage);
            sdkExplorerAbstraction.Initialize(exportProvider, printMessage);
            msBuildFinderAbstraction.Initialize(exportProvider, printMessage);
            passwordProviderAbstraction.Initialize(exportProvider, printMessage);
            ILog log = new LogTracer(printMessage);
            exportProvider.AddInstance(Substitute.For<IProgressVisualizer>());
            exportProvider.AddInstance(log);
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new DiModule(exportProvider, autoActivatedComponents));
            buildAction?.Invoke(builder);
            Host = builder.Build();
            
            Initialized = true;
            
            exceptionHandlerAbstraction.UserInterface = Host.ResolveOptional<IUserInterface>();
        }

        public void Dispose()
        {
            if (Initialized)
            {
                Host?.Dispose();
                fileSystemAbstraction?.Dispose();
                downloadServiceAbstraction?.Dispose();
                processManagerAbstraction?.Dispose();
                userInterfaceAbstraction?.Dispose();
            }
        }

        private string knownProjectName;

        public enum ProjectType
        {
            PlmProject,
            AcfProject,
            ConsumableLibrary,
            SharedNativeProject,
            TestProject
        }

        public async Task CreateProject(string projectName = null, string componentName = null, string programName = null,
            bool forced = false, string folder = null, ProjectType type = ProjectType.PlmProject, bool empty = false)
        {

            string[] args;
            switch (type)
            {
                case ProjectType.PlmProject:
                    args = new string[] { "new", "project" };
                    break;
                case ProjectType.AcfProject:
                    args = new string[] { "new", "acfproject" };
                    break;
                case ProjectType.ConsumableLibrary:
                    args = new string[] { "new", "consumablelibrary" };
                    break;
                case ProjectType.SharedNativeProject:
                    args = new string[] { "new", "snproject" };
                    break;
                case ProjectType.TestProject:
                    args = new string[] { "new", "testproject" };
                    break;
                default:
                    true.ShouldBeFalse($"Project type {type} is unknown.");
                    args = Array.Empty<string>();
                    break;
            }

            if (!string.IsNullOrEmpty(projectName))
            {
                args = args.Append("-n").Append(projectName).ToArray();
                knownProjectName = projectName;
            }
            if (forced)
            {
                args = args.Append("-f").ToArray();
            }
            if (!string.IsNullOrEmpty(componentName))
            {
                args = args.Append("--component").Append(componentName).ToArray();
            }
            if (!string.IsNullOrEmpty(programName))
            {
                args = args.Append("--program").Append(programName).ToArray();
            }
            if (!string.IsNullOrEmpty(folder))
            {
                args = args.Append("--output").Append(Path.Combine(folder)).ToArray();
            }
            if (empty)
            {
                args = args.Append("--empty").ToArray();
            }
            await CommandLineParser.Parse(args);
        }

        internal void CheckProjectCreated(string projectName, bool empty = false)
        {
            CheckProjectCreatedInFolder(projectName, projectName, empty);
        }

        internal void CheckProjectCreatedInFolder(string projectName, string folder, bool empty = false)
        {
            folder ??= string.Empty;
            string path = Path.Combine(folder, projectFileName);
            CheckFileCreated(folder, path);

            path = Path.Combine(folder, "CMakeLists.txt");
            CheckFileCreated(folder, path);

            path =  Path.Combine(folder, "src", projectName + "Program.cpp");
            CheckFileCreated(folder, path, empty);
            path = Path.Combine(folder, "src", projectName + "Component.cpp");
            CheckFileCreated(folder, path, empty);
            
            CheckUserInformed("Successfully created template", "Message that project was created successfully expected");
        }

        private void CheckFileCreated(string folder, string file, bool checkNotExistent = false)
        {
            using Stream fileContent = fileSystemAbstraction.Open(file);
            string msg;
            if (checkNotExistent)
            {
                msg = string.IsNullOrEmpty(folder) ? $"{file} file was expected not to exist"
                    : $"{file} file was expected not to exist in folder {folder}";
                fileContent.ShouldBeNull(msg);
            }
            else
            {
                msg = string.IsNullOrEmpty(folder) ? $"{file} file was expected to exist"
                    : $"{file} file was expected to exist in folder {folder}";
                fileContent.ShouldNotBeNull(msg);
                fileContent.Flush();
                fileContent.Seek(0, SeekOrigin.Begin);
            }
        }

        internal void CheckProjectCreatedTwice()
        {
            IEnumerable<string> message = userInterfaceAbstraction.Informations.Where(s => s.Contains("Successfully created template"));
            message.Count().ShouldBe(2, "project was expected to be created twice");
        }

        internal void CheckUserInformedOfError(Type exceptionType)
        {
            exceptionHandlerAbstraction.WasExceptionThrown(exceptionType).ShouldBeTrue($"reported error {exceptionType} was expected.");
        }

        internal void CheckUserInformedOfError(string searchstring, string reason)
        {
            string message = userInterfaceAbstraction.Errors.FirstOrDefault(s => s.Contains(searchstring, StringComparison.InvariantCultureIgnoreCase));
            message.ShouldNotBeNull(reason);
        }

        internal void CheckUserInformed(string searchstring, string reason)
        {
            string message = userInterfaceAbstraction.Informations.FirstOrDefault(s => s.Contains(searchstring, StringComparison.InvariantCultureIgnoreCase));
            message.ShouldNotBeNull(reason);
        }

        internal void CheckUserInformedOfWarning(string searchstring, string reason)
        {
            string message = userInterfaceAbstraction.Warnings.FirstOrDefault(s => s.Contains(searchstring, StringComparison.InvariantCultureIgnoreCase));
            message.ShouldNotBeNull(reason);
        }

        internal void CheckTargetSupported(string[] targets, bool sorted)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            using (Stream fileContent = fileSystemAbstraction.Open(Path.Combine(knownProjectName, projectFileName)))
            {
                fileContent.ShouldNotBeNull($"{projectFileName} file was expected to exist in folder {knownProjectName}");
                ProjectMetaFileChecker.Check(fileContent)
                                      .SupportsTargetTypes(targets, sorted);
            }
        }

        internal void CheckCppAndHppFilesExist()
        {
            CheckComponentHasName("RootComponent");

            CheckProgramHasName("RootProgram");
        }

        internal void CheckComponentHasName(string componentname)
        {
            string path = GetPathOfFile($"{componentname}.cpp", Constants.SourceFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.Contains($"void {componentname}::Initialize()").ShouldBeTrue($"Content 'void {componentname}::Initialize()' was expected to exist. Actual content{Environment.NewLine}{content}");
                }
            }

            path = GetPathOfFile($"{componentname}.hpp", Constants.SourceFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.Contains($"class {componentname} : public ComponentBase").ShouldBeTrue($"Content 'class {componentname} : public ComponentBase' was expected to exist. Actual content{Environment.NewLine}{content}");
                }
            }
        }

        internal void CheckIsAcfComponent(string componentname)
        {
            string path = GetPathOfFile($"{componentname}.hpp", Constants.SourceFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.Contains("public MetaComponentBase").ShouldBeTrue($"Content 'public MetaComponentBase' was expected to exist. Actual content{Environment.NewLine}{content}");
                }
            }
        }

        internal void CheckProgramHasName(string programName)
        {
            string path = GetPathOfFile($"{programName}.cpp", Constants.SourceFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.Contains($"{programName}::Execute()").ShouldBeTrue();
                }
            }

            path = GetPathOfFile($"{programName}.hpp", Constants.SourceFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.ShouldContain($"class {programName} : public ProgramBase");
                }
            }
        }

        internal void CheckSourceEntitiesNamespace(string ns, string entityName)
        {
            CheckEntitiesNamespace(ns, $"{entityName}.{Constants.ClassExtension}", Constants.SourceFolderName);
            CheckEntitiesNamespace(ns, $"{entityName}.{Constants.HeaderExtension}", Constants.SourceFolderName);
        }

        internal void CheckGeneratedComponentsNamespace(string ns, string entityName)
        {
            CheckEntitiesNamespace(ns, $"{entityName}.meta.{Constants.ClassExtension}", Constants.IntermediateFolderName,
                                   Constants.GeneratedCodeFolderName);
        }

        private void CheckEntitiesNamespace(string ns, string fileName, params string[] parts)
        {
            string path = GetPathOfFile(fileName, parts);

            using (Stream fileContent = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(fileContent))
            {
                string content = reader.ReadToEnd();
                string[] namespaces = ns.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                content.ShouldContain($"namespace {namespaces.Aggregate((i, j) => i + $" {{ namespace {j}")}{Environment.NewLine}{{");
            }
        }

        internal void CheckCodeEntityCreatedInDefaultNamespace(string fileName)
        {
            fileSystemAbstraction.FindFile(ref fileName).ShouldBeTrue($"The {fileName} file is expected to exist in workspace.");

            using (Stream fileContent = fileSystemAbstraction.Open(fileName))
            using (StreamReader reader = new StreamReader(fileContent))
            {
                string content = reader.ReadToEnd();
                content.ShouldContain($"namespace Root{Environment.NewLine}{{");
                content.ShouldNotContain($"{{ namespace");
            }
        }

        internal void CheckLibraryIsGenerated(string[] components, bool useCommonTypeName = true)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string projectName = knownProjectName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            string libName = $"{projectName}Library";
            libName.ShouldNotBeNullOrEmpty("Library name cannot be determined.");
            string path = GetPathOfGeneratedFile($"{libName}.{Constants.ClassExtension}", Constants.GeneratedCodeFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    string content = reader.ReadToEnd();
                    content.ShouldContain($"{libName}::{libName}(AppDomain& appDomain)");
                    foreach (string component in components)
                    {
                        if(useCommonTypeName)
                        {
                         content.Contains($"this->componentFactory.AddFactoryMethod(CommonTypeName<::{projectName}::{component}>(), &::{projectName}::{component}::Create);")
                               .ShouldBeTrue($"Could not find line 'this->componentFactory.AddFactoryMethod" +
                                                $"(CommonTypeName<::{projectName}::{component}>(), &::{projectName}::{component}::Create);' " +
                                                $"in {libName}.{Constants.ClassExtension}");

                        }
                        else
                        {
                            content.Contains($"this->componentFactory.AddFactoryMethod(TypeName<::{projectName}::{component}>(), &::{projectName}::{component}::Create);")
                               .ShouldBeTrue($"Could not find line 'this->componentFactory.AddFactoryMethod" +
                                                $"(TypeName<::{projectName}::{component}>(), &::{projectName}::{component}::Create);' " +
                                                $"in {libName}.{Constants.ClassExtension}");

                        }

                    }
                }
            }

            path = GetPathOfGeneratedFile($"{libName}.{Constants.HeaderExtension}", Constants.GeneratedCodeFolderName);
            using (Stream fileContent = fileSystemAbstraction.Open(path))
            {
                using (StreamReader reader = new StreamReader(fileContent))
                {
                    ;
                    string content = reader.ReadToEnd();
                    content.Contains($"class {libName} : public MetaLibraryBase").ShouldBeTrue();
                    content.Contains($", public Singleton<{libName}>").ShouldBeTrue();
                }
            }
        }

        internal void CheckProviderIsGeneratedForComponent(string component, string[] programs)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string path = GetPathOfGeneratedFile($"{component}ProgramProvider.{Constants.HeaderExtension}", Constants.GeneratedCodeFolderName);
            using (Stream fileStream = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string content = reader.ReadToEnd();
                content.ShouldContain($"class {component}ProgramProvider", customMessage: "Component name should have been replaced");
                content.ShouldNotContain("namespace)", customMessage: "namespace should have been replaced");
            }

            path = GetPathOfGeneratedFile($"{component}ProgramProvider.{Constants.ClassExtension}", Constants.GeneratedCodeFolderName);
            using (Stream fileStream = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string content = reader.ReadToEnd();
                content.ShouldNotContain("namespace)", customMessage: "namespace should have been replaced");
                content.ShouldContain($"IProgram::Ptr {component}", customMessage: "Component name should have been replaced");
                foreach (string program in programs)
                {
                    //TODO: seems that this check did never run before
                    //content.ShouldContain($"if (programType == \"{program}\"){{ return std::make_shared<{program}>(programName); }}");
                }
            }
        }


        public void LoadProject(string project)
        {
            knownProjectName = project;
            fileSystemAbstraction.Load(project);
        }

        public void LoadLibrary(string library, string directory)
        {
            fileSystemAbstraction.LoadInto(library, Path.Combine(knownProjectName, directory));
        }

        public void FileAddedOrChanged(int countChanged)
        {
            fileSystemAbstraction.ChangedFiles.Concat(fileSystemAbstraction.CreatedFiles)
                                 .Count().ShouldBe(countChanged, "there should be the specified number of created or changed files");
        }

        public void ExistingFilesDeleted(int countDeleted)
        {
            fileSystemAbstraction.DeletedFiles.Count().ShouldBe(countDeleted, "there should be the specified number of initially deleted files");
        }

        public async Task GenerateAll(bool addPath)
        {
            List<string> arguments = new List<string>(new[] { "generate", "all" });
            if (addPath)
            {
                arguments.Add("-p");
                arguments.Add(fileSystemAbstraction.FileExists($"{knownProjectName}/plcnext.proj", string.Empty)
                                  ? $"{knownProjectName}/plcnext.proj"
                                  : $"{knownProjectName}");
            }
            await CommandLineParser.Parse(arguments.ToArray());
        }

        public async Task GenerateMeta(bool addPath, string[] sourceDirectories = null, string[] includes = null, 
                                       bool autoDetection = true, bool noDatatypesWorksheet = false)
        {
            List<string> arguments = new List<string>(new[] { "generate", "config" });
            if (addPath)
            {
                arguments.Add("-p");
                arguments.Add(fileSystemAbstraction.FileExists($"{knownProjectName}/plcnext.proj", string.Empty)
                                  ? $"{knownProjectName}/plcnext.proj"
                                  : $"{knownProjectName}");
            }

            if (sourceDirectories?.Any() == true)
            {
                arguments.Add("-s");
                arguments.Add(string.Join(",", sourceDirectories));
            }

            if (includes?.Any() == true)
            {
                arguments.Add("-i");
                arguments.Add(string.Join(",", includes));
            }

            if (!autoDetection)
            {
                arguments.Add("-n");
            }

            if(noDatatypesWorksheet)
            {
                arguments.Add("--no-datatypes-worksheet");
            }

            await CommandLineParser.Parse(arguments.ToArray());
        }

        public async Task GenerateCode(bool addPath, string[] sourceDirectories = null, string[] includes = null, bool autoDetection = true)
        {
            List<string> arguments = new List<string>(new[] {"generate", "code"});
            if (addPath)
            {
                arguments.Add("-p");
                arguments.Add(fileSystemAbstraction.FileExists($"{knownProjectName}/plcnext.proj", string.Empty)
                                  ? $"{knownProjectName}/plcnext.proj"
                                  : $"{knownProjectName}");
            }

            if (sourceDirectories?.Any() == true)
            {
                arguments.Add("-s");
                arguments.Add(string.Join(",", sourceDirectories));
            }

            if (includes?.Any() == true)
            {
                arguments.Add("-i");
                arguments.Add(string.Join(",", includes));
            }

            if (!autoDetection)
            {
                arguments.Add("-n");
            }

            await CommandLineParser.Parse(arguments.ToArray());
        }

        public async Task Build(bool addPath = true, string target = null, string version = null, string buildType = null, string cmakeArgs = null)
        {
            List<string> arguments = new List<string>(new[] { "build", "--verbose"});
            if (addPath)
            {
                arguments.Add("-p");
                arguments.Add(knownProjectName);
            }

            if(!string.IsNullOrEmpty(target))
            {
                string targetName = string.IsNullOrEmpty(version) ? target : $"{target},{version}";
                arguments.Add("-t");
                arguments.Add(targetName);
            }

            if (!string.IsNullOrEmpty(buildType))
            {
                arguments.Add("-b");
                arguments.Add(buildType);
            }

            if (!string.IsNullOrEmpty(cmakeArgs))
            {
                arguments.Add("--");
                arguments.Add(cmakeArgs);
            }

            await CommandLineParser.Parse(arguments.ToArray());
        }
        
        public void CheckTypemetaFile(TypemetaStructure[] typemetaStructures, bool structureIsComplete = false)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string path = GetPathOfGeneratedFile($"{knownProjectName}.{Constants.TypemetaExtension}", Constants.MetadataFolderName);
            using (Stream fileStream = fileSystemAbstraction.Open(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                MetaConfigurationDocument document = (MetaConfigurationDocument)serializer.Deserialize(fileStream);
                TypesDefinition typesDefinition = document.Item as TypesDefinition;
                typesDefinition.ShouldNotBeNull("metadata content should be a TypesDefinition");

                if (typemetaStructures == null)
                {
                    int count = typesDefinition?.Items?.Length ?? 0;
                    count.ShouldBe(0, $"no structure definition was expected.");
                    return;
                }

                if (structureIsComplete)
                {
                    typesDefinition.Items?.OfType<StructTypeDefinition>().Count().ShouldBe(typemetaStructures.OfType<StructTypemetaStructure>().Count());
                    typesDefinition.Items?.OfType<EnumerationTypeDefinition>().Count().ShouldBe(typemetaStructures.OfType<EnumTypemetaStructure>().Count());
                }

                foreach (StructTypemetaStructure structure in typemetaStructures.OfType<StructTypemetaStructure>())
                {
                    StructTypeDefinition definition = typesDefinition?.Items?.OfType<StructTypeDefinition>()
                                                                     .FirstOrDefault(s => s.type == structure.TypeName);
                    definition.ShouldNotBeNull($"structure {structure.TypeName} should be inside xml document. Available definitions:{Environment.NewLine}{ObjectToString(typesDefinition)}");
                    definition.Fields.Count().ShouldBe(structure.TypeMembers.Count(),
                                                             $"defined members count should match. Available members:{Environment.NewLine}{ObjectToString(definition)}");
                    foreach (TypeMember member in structure.TypeMembers)
                    {
                        FieldDefinition fieldDefinition = definition.Fields.FirstOrDefault(f => f.name == member.Name);
                        fieldDefinition.ShouldNotBeNull($"fieldDefinition {member.Name} expected in structure {ObjectToString(definition)}");

                        if (member.MultiplicityUsed && (!string.IsNullOrEmpty(member.Multiplicity) || !string.IsNullOrEmpty(fieldDefinition.dimensions)))
                        {
                            fieldDefinition.dimensions.ShouldBe(member.Multiplicity);
                        }
                        fieldDefinition.type.ShouldBe(member.Type);
                        if (member.AttributesUsed && (!string.IsNullOrEmpty(member.Attributes) || !string.IsNullOrEmpty(fieldDefinition.attributes)))
                        {
                            fieldDefinition.attributes.ShouldBe(member.Attributes);
                        }
                    }
                }

                foreach (EnumTypemetaStructure @enum in typemetaStructures.OfType<EnumTypemetaStructure>())
                {
                    EnumerationTypeDefinition definition = typesDefinition?.Items?.OfType<EnumerationTypeDefinition>()
                                                                           .FirstOrDefault(e => e.type == @enum.TypeName);
                    definition.baseType.ShouldBe(@enum.BaseType);
                    definition.Symbols.Count().ShouldBe(@enum.Symbols.Count(),
                                                             $"defined members count should match. Available members:{Environment.NewLine}{ObjectToString(definition)}");
                    definition.ShouldNotBeNull($"enum {@enum.TypeName} should be inside xml document. Available definitions:{Environment.NewLine}{ObjectToString(typesDefinition)}");
                    foreach (EnumSymbol symbol in @enum.Symbols)
                    {
                        EnumTypeElement symbolDefinition = definition.Symbols.FirstOrDefault(e => e.name == symbol.Name);
                        symbolDefinition.ShouldNotBeNull($"fieldDefinition {symbol.Name} expected in enum {ObjectToString(definition)}");
                        symbolDefinition.value.ShouldBe(symbol.Value);
                    }
                }
            }

            string ObjectToString(object structTypeDefinition)
            {
                return JsonConvert.SerializeObject(structTypeDefinition, Formatting.Indented,
                                                   new JsonSerializerSettings
                                                   {
                                                       ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                                   });
            }
        }

        public void ThrowOnFileAccess(string path)
        {
            fileSystemAbstraction.ThrowOnAccess(path);
        }

        public void StartProcessThrowsError()
        {
            processManagerAbstraction.ThrowError = true;
        }

        public void ExitProcessWithErrorForCommand(string command)
        {
            processManagerAbstraction.ExitWithErrorForCommand = command;
        }

        public void CheckForError(ErrorInformation error)
        {
            string errors = userInterfaceAbstraction.Errors.Aggregate(string.Empty, (s, s1) => $"{s}{Environment.NewLine}{s1}");
            userInterfaceAbstraction.Errors.Where(e => e.Contains(error.ErrorCode))
                                    .Where(e => e.Contains(error.Filename))
                                    .Any(e => e.Contains($"({error.Line},{error.Column}):"))
                                    .ShouldBeTrue($"error {error.ToFullString()} was expected. Available errors:{errors}");
        }

        public void SetCurrentDirectory(string path)
        {
            fileSystemAbstraction.SetCurrentDirectory(path);
        }

        internal void CheckBuildStartedAndEnded()
        {
            CheckUserInformed("Starting build", "build should have started");
            CheckUserInformed("Successfully built the project", "build should have finished successfully");
        }

        public async Task CreateComponent(bool forced, bool addPath, string name = null, string path = null)
        {
            string[] args = new string[] { "new", "component" };
            if (!string.IsNullOrEmpty(name))
            {
                args = args.Append("-n").ToArray();
                args = args.Append(name).ToArray();
            }
            if (addPath)
            {
                args = args.Append("-p").ToArray();
                args = args.Append(fileSystemAbstraction.FileExists($"{knownProjectName}/plcnext.proj", string.Empty)?$"{knownProjectName}/plcnext.proj": $"{knownProjectName}").ToArray();
            }
            if (forced)
            {
                args = args.Append("-f").ToArray();
            }
            if (!string.IsNullOrEmpty(path))
            {
                args = args.Append("-o").ToArray();
                args = args.Append(path).ToArray();
            }
            await CommandLineParser.Parse(args);
        }

        public async Task CreateProgram(bool forced, bool addPath, string name = null, string component = null, string path = null, string[] sourceFolders = null)
        {
            string[] args = new string[] { "new", "program" };
            if (!string.IsNullOrEmpty(name))

                args = args.Append("-n").Append(name).ToArray();

            if (addPath)

                args = args.Append("-p").Append(fileSystemAbstraction.FileExists($"{knownProjectName}/plcnext.proj", string.Empty)?$"{knownProjectName}/plcnext.proj": $"{knownProjectName}").ToArray();

            if (forced)

                args = args.Append("-f").ToArray();

            if (!string.IsNullOrEmpty(component))

                args = args.Append("--component").Append(component).ToArray();

            if (!string.IsNullOrEmpty(path))

                args = args.Append("-o").Append(path).ToArray();

            if (sourceFolders != null)
            {
                args = args.Append("--sources").Append(string.Join(",", sourceFolders)).ToArray();
            }

            await CommandLineParser.Parse(args);
        }

        public void CheckCodeEntityCreated()
        {
            FileAddedOrChanged(2);
        }

        public void CheckGeneratedComponentCodeFiles(string component, string[] ports = null, bool shouldExist = true)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string path = GetPathOfGeneratedFile($"{component}.meta.{Constants.ClassExtension}", shouldExist, Constants.GeneratedCodeFolderName);
            if (shouldExist)
            {
                using (Stream fileStream = fileSystemAbstraction.Open(path))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string content = reader.ReadToEnd();
                    content.ShouldNotContain($"$(componentFile)", customMessage: "Component file path should have been replaced");
                    content.ShouldNotContain("$(namespace)", customMessage: "namespace should have been replaced");
                    content.ShouldNotContain("$(libraryName)", customMessage: "library name should have been replaced");
                    content.ShouldNotContain("$(portField)", customMessage: "portField should have been replaced");
                    content.ShouldNotContain("$(portName)", customMessage: "portName should have been replaced");
                    foreach (string port in ports)
                    {
                        content.ShouldContain($"dataInfoProvider.AddRoot(\"{port}\", this->{port});",
                                                 customMessage: $"port {port} should have been registered");
                    }
                }
            }
        }

        private string GetPathOfGeneratedFile(string fileName, bool shouldExist, params string[] parts)
        {
            return GetPathOfFile(fileName, shouldExist, new[] { Constants.IntermediateFolderName }.Concat(parts).ToArray());
        }

        private string GetPathOfGeneratedFile(string fileName, params string[] parts)
        {
            return GetPathOfGeneratedFile(fileName, true, parts);
        }

        private string GetPathOfFile(string fileName, params string[] parts)
        {
            return GetPathOfFile(fileName, true, parts);
        }

        private string GetPathOfFile(string fileName, bool shouldExist, params string[] parts)
        {
            string path = parts.Concat(new[] { fileName }).Aggregate(string.Empty, Path.Combine);
            if (!fileSystemAbstraction.FindFile(ref path))
            {
                path = Path.Combine(knownProjectName, path);
            }

            if (shouldExist)
            {
                fileSystemAbstraction.FindFile(ref path).ShouldBeTrue($"The {fileName} file is expected to exist in path {path}.");
            }
            else
            {
                fileSystemAbstraction.FindFile(ref path).ShouldBeFalse($"The {fileName} file is expected NOT to exist in path {path}.");
            }
            return path;
        }

        public void CheckProgmetaFiles(ProgmetaData[] progmetaDatas)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            foreach (ProgmetaData progmetaData in progmetaDatas)
            {
                string path = GetPathOfGeneratedFile($"{progmetaData.ProgramName}.{Constants.ProgmetaExtension}", new[] { Constants.MetadataFolderName }.Concat(progmetaData.Path).ToArray());
                using (Stream fileStream = fileSystemAbstraction.Open(path))
                {
                    //StreamReader reader = new StreamReader(fileStream);
                    //string content = reader.ReadToEnd();
                    XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                    MetaConfigurationDocument document = (MetaConfigurationDocument)serializer.Deserialize(fileStream);
                    ProgramDefinition programDefinition = document.Item as ProgramDefinition;
                    programDefinition.ShouldNotBeNull("metadata content should be a ProgramDefinition");

                    IEnumerable<PortDefinition> ports = programDefinition.Items.SelectMany(pl => pl.Port??Array.Empty<PortDefinition>()).Where(p => p != null).ToArray();
                    ports.Count().ShouldBe(progmetaData.Portmetas.Count(), $"defined port count should match. Available definitions:{Environment.NewLine}{DefinitionsToString(ports)}");
                    foreach (Portmeta portmeta in progmetaData.Portmetas)
                    {
                        PortDefinition portDefinition = ports.FirstOrDefault(p => p.name == portmeta.Name);
                        portDefinition.ShouldNotBeNull($"port definition {portmeta.Name} not found. Available definitions:{Environment.NewLine}{DefinitionsToString(ports)}");

                        portDefinition.attributes.ShouldBe(portmeta.Attributes, "attributes should match 1:1");
                        portDefinition.type.ShouldBe(portmeta.Type);

                        if (portmeta.MultiplicityUsed && (!string.IsNullOrEmpty(portmeta.Multiplicity) || !string.IsNullOrEmpty(portDefinition.dimensions)))
                        {
                            portDefinition.dimensions.ShouldBe(portmeta.Multiplicity);
                        }
                    }
                }
            }
        }
        string DefinitionsToString(IEnumerable<PortDefinition> ports)
        {
            return JsonConvert.SerializeObject(ports, Formatting.Indented,
                                               new JsonSerializerSettings
                                               {
                                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                               });
        }

        public void CheckCompmetaFiles(CompmetaData[] compmetaDatas)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            foreach (CompmetaData compmetaData in compmetaDatas)
            {
                string path = GetPathOfGeneratedFile($"{compmetaData.ComponentName}.{Constants.CompmetaExtension}", new[] { Constants.MetadataFolderName }.Concat(compmetaData.Path).ToArray());
                using (Stream fileStream = fileSystemAbstraction.Open(path))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                    MetaConfigurationDocument document = (MetaConfigurationDocument)serializer.Deserialize(fileStream);
                    ComponentDefinition componentDefinition = document.Item as ComponentDefinition;
                    componentDefinition.ShouldNotBeNull("metadata content should be a ComponentDefinition");

                    IEnumerable<IncludeDefinition> includes = componentDefinition.Items.OfType<IncludeListDefinition>().SingleOrDefault().Include;
                    if (includes == null)
                    {
                        compmetaData.Programs.Count().ShouldBe(0, "defined include count should match.");
                    }
                    else
                    {
                        includes.Count().ShouldBe(compmetaData.Programs.Count(), "defined include count should match.");

                        foreach (string program in compmetaData.Programs)
                        {
                            IncludeDefinition includeDefinition = includes.FirstOrDefault(i => i.path.Equals($"{program}/{program}.{Constants.ProgmetaExtension}"));
                            includeDefinition.ShouldNotBeNull($"include definition for program {program} not found.");
                        }
                    }

                    IEnumerable<PortDefinition> ports = componentDefinition.Items.OfType<PortListDefinition>().SingleOrDefault().Port;
                    if (ports == null)
                    {
                        compmetaData.Portmetas.Count().ShouldBe(0, "defined port count should match.");
                    }
                    else
                    {
                        ports.Count().ShouldBe(compmetaData.Portmetas.Count(), $"defined port count should match. Available definitions:{Environment.NewLine}{DefinitionsToString(ports)}");

                        foreach (Portmeta portmeta in compmetaData.Portmetas)
                        {
                            PortDefinition portDefinition = ports.FirstOrDefault(p => p.name == portmeta.Name);
                            portDefinition.ShouldNotBeNull($"port definition {portmeta.Name} not found. Available definitions:{Environment.NewLine}{DefinitionsToString(ports)}");

                            portDefinition.attributes.ShouldBe(portmeta.Attributes, "attributes should match 1:1");
                            portDefinition.type.ShouldBe(portmeta.Type);

                            if (portmeta.MultiplicityUsed && (!string.IsNullOrEmpty(portmeta.Multiplicity) || !string.IsNullOrEmpty(portDefinition.dimensions)))
                            {
                                portDefinition.dimensions.ShouldBe(portmeta.Multiplicity);
                            }
                        }
                    }
                }
            }
        }

        public void CheckGeneratedLibmeta(string[] components)
        {
            string path = GetPathOfGeneratedFile($"{knownProjectName}.{Constants.LibmetaExtension}", Constants.MetadataFolderName);
            using (Stream fileStream = fileSystemAbstraction.Open(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                MetaConfigurationDocument document = (MetaConfigurationDocument)serializer.Deserialize(fileStream);
                LibraryDefinition libraryDefinition = document.Item as LibraryDefinition;
                libraryDefinition.ShouldNotBeNull("metadata content should be a LibraryDefinition");
                libraryDefinition.applicationDomain.ShouldBe(ApplicationDomainEnumeration.CPLUSPLUS);
                libraryDefinition.File.path.ShouldBe($"lib{knownProjectName}.{Constants.SharedObjectExtension}");
                libraryDefinition.TypeIncludes.ShouldHaveSingleItem();
                libraryDefinition.ComponentIncludes.Count().ShouldBe(components.Length);
                libraryDefinition.ComponentIncludes.Select(i => Path.GetFileNameWithoutExtension(i.path)).ToArray()
                                 .ShouldBeEquivalentTo(components);
            }
        }

        public void CheckDependenciesOfDeployedLibmeta(string projectName, string[] dependencies)
        {
            string path = GetPathOfFile($"{projectName}.{Constants.LibmetaExtension}", Constants.LibraryFolderName );
            using (Stream fileStream = fileSystemAbstraction.Open(path)) 
            { 
                XmlSerializer serializer = new XmlSerializer(typeof(MetaConfigurationDocument));
                MetaConfigurationDocument document = (MetaConfigurationDocument)serializer.Deserialize(fileStream);
                LibraryDefinition libraryDefinition = document.Item as LibraryDefinition;
                libraryDefinition.ShouldNotBeNull("metadata content should be a LibraryDefinition");
                libraryDefinition.applicationDomain.ShouldBe(ApplicationDomainEnumeration.CPLUSPLUS);
                libraryDefinition.TypeIncludes.ShouldHaveSingleItem();
                libraryDefinition.Dependencies.Count().ShouldBe(dependencies.Length);
                libraryDefinition.Dependencies.Select(i => i.path).ToArray()
                                 .ShouldBeEquivalentTo(dependencies);
            }
        }

        public void CheckCreatedAcfConfig(string ns, string componentname)
        {
            string path = GetPathOfFile($"{ns}Library.acf.config", Constants.SourceFolderName);
            using (Stream fileStream = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string content = reader.ReadToEnd();
                content.Contains("<AcfConfigurationDocument").ShouldBeTrue($"Content '<AcfConfigurationDocument' was expected to exist. Actual content{Environment.NewLine}{content}");
                content.Contains($"<Component name=\"{componentname}1\" type=\"{ns}.{componentname}\" library=\"{ns}")
                    .ShouldBeTrue($"Content '<Component name=\"{componentname}1\" type=\"{ns}.{componentname}\" library=\"{ns}' was expected to exist. Actual content{Environment.NewLine}{content}");
            }
        }

        public void CheckDeployedAcfConfig(string ns, string componentname, string deployPath)
        {
            string path = GetPathOfFile($"{ns}Library.acf.config", deployPath);
            fileSystemAbstraction.FileExists(path).ShouldBeTrue($"acf.config expected in {path}");
        }

        public void CheckTypemetaMethod(string compareFile)
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string libraryFileName = knownProjectName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            libraryFileName = $"{libraryFileName}Library";
            libraryFileName.ShouldNotBeNullOrEmpty("Library name cannot be determined.");
            string path = GetPathOfGeneratedFile($"{libraryFileName}.meta.{Constants.ClassExtension}", Constants.GeneratedCodeFolderName);
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(stream))
            using (Stream resourceStream = assembly.GetManifestResourceStream($"Test.PlcNext.Deployment.TestResults.{compareFile}"))
            using (StreamReader resourceReader = new StreamReader(resourceStream))
            {
                //StringBuilder builder = new StringBuilder(reader.ReadToEnd());
                reader.EndOfStream.ShouldBeFalse("content already read");
                while (!reader.EndOfStream)
                {
                    string actualContent = reader.ReadLine();
                    string expectedContent = resourceReader.EndOfStream ? string.Empty : resourceReader.ReadLine();
                    actualContent.ShouldBe(expectedContent);
                }
            }
        }

        public void CheckDatatypeWorksheet(string compareFile, string projectName = "")
        {
            if (string.IsNullOrEmpty(projectName))
            {
                projectName = knownProjectName;
            }
            projectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string libraryName = projectName.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            libraryName.ShouldNotBeNullOrEmpty("Library name cannot be determined.");
            string path = GetPathOfGeneratedFile($"{libraryName}DataTypes.dt", Constants.MetadataFolderName);
            Assembly assembly = Assembly.GetExecutingAssembly();
            using(Stream stream = fileSystemAbstraction.Open(path))
            using(StreamReader reader = new StreamReader(stream))
            using(Stream resourceStream = assembly.GetManifestResourceStream($"Test.PlcNext.Deployment.TestResults.{compareFile}"))
            using(StreamReader resourceReader = new StreamReader(resourceStream))
            {
                //StringBuilder builder = new StringBuilder(reader.ReadToEnd());
                reader.EndOfStream.ShouldBeFalse("file should not be empty or already read.");
                int line = 0;
                while(!reader.EndOfStream)
                {
                    line++;
                    string actualContent = reader.ReadLine();
                    string expectedContent = resourceReader.EndOfStream ? string.Empty : resourceReader.ReadLine();
                    actualContent.ShouldBe(expectedContent, $"lines {line} should be equal");
                }
            }
        }

        public void CheckCMakeFile()
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string path = GetPathOfFile("CMakeLists.txt");
            using (Stream stream = fileSystemAbstraction.Open(path))
            using (StreamReader reader = new StreamReader(stream))
            {
                string content = reader.ReadToEnd();
                content.ShouldContain($"project({knownProjectName})");
                content.ShouldNotMatch(@"\$\(.*\)");
            }
        }

        public void RemoveSdk()
        {
            fileSystemAbstraction.RemoveSdk();
        }

        public async Task ChangeSetting(string setting, string value, SettingChange change)
        {
            if (string.IsNullOrEmpty(value))
            {
                switch (change)
                {
                    case SettingChange.Add:
                        await CommandLineParser.Parse("set", "setting", setting, "--add");
                        break;
                    case SettingChange.Remove:
                        await CommandLineParser.Parse("set", "setting", setting, "--remove");
                        break;
                    case SettingChange.Clear:
                        await CommandLineParser.Parse("set", "setting", setting, "--clear");
                        break;
                    default:
                        await CommandLineParser.Parse("set", "setting", setting);
                        break;
                }
            }
            else
            {
                switch (change)
                {
                    case SettingChange.Add:
                        await CommandLineParser.Parse("set", "setting", setting, value, "--add");
                        break;
                    case SettingChange.Remove:
                        await CommandLineParser.Parse("set", "setting", setting, value, "--remove");
                        break;
                    case SettingChange.Clear:
                        await CommandLineParser.Parse("set", "setting", setting, value, "--clear");
                        break;
                    default:
                        await CommandLineParser.Parse("set", "setting", setting, value);
                        break;
                }
            }
        }

        public async Task CheckSetting(string setting, params string[] values)
        {
            await CommandLineParser.Parse("get", "setting", "--all");
            JObject settings = JObject.Parse(userInterfaceAbstraction.Informations.LastOrDefault());
            settings.ContainsKey("setting").ShouldBeTrue($"Root element should be 'setting'");
            settings = (JObject) settings["setting"];
            settings.ContainsKey(setting).ShouldBeTrue($"setting {setting} is supposed to exist in root level. Json:{Environment.NewLine}{settings}");
            string[] actualValues = settings[setting].HasValues
                                        ? settings[setting].Values<string>().ToArray()
                                        : new[] { settings[setting].ToString() };
            if (actualValues.Length == 1 && actualValues[0] == "[]")
            {
                actualValues = new string[0];
            }
            actualValues.ShouldBeEquivalentTo(values);
        }

        public async Task UpdateCli(Version version = null, string proxy = "")
        {
            if (version == null)
            {
                if (string.IsNullOrEmpty(proxy))
                {
                    await CommandLineParser.Parse("update", "cli");
                }
                else
                {
                    await CommandLineParser.Parse("update", "cli", "-p", proxy);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(proxy))
                {
                    await CommandLineParser.Parse("update", "cli", "-v", version.ToString(3));
                }
                else
                {
                    await CommandLineParser.Parse("update", "cli", "-p", proxy, "-v", version.ToString(3));
                }
            }
        }

        public void CheckNewCliVersion(Version version)
        {
            VirtualFile mainAssembly = fileSystemAbstraction.FileSystem.GetFile(Path.Combine(AssemblyDirectory, "plcncli.dll"));
            using (Stream stream = mainAssembly.OpenRead())
            {
                AssemblyDefinition definition = AssemblyDefinition.ReadAssembly(stream);
                definition.Name.Version.ShouldBe(version);
            }
        }

        private static string AssemblyDirectory
        {
            get
            {
                string location = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(location);
            }
        }

        public void CheckNewSettingWasChanged(string setting, string value)
        {
            processManagerAbstraction.CommandExecuted("plcncli", "set", "setting",
                                                      setting, value).ShouldBeTrue("command to change setting was " +
                                                                                    "expected to have been executed.");
        }

        public void RemoveFromWebServer(string path)
        {
            downloadServiceAbstraction.RemoveFromServer(path);
        }

        public void ChangeFileOnWebServer(string newContentResourceKey, string path)
        {
            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(newContentResourceKey))
            {
                downloadServiceAbstraction.ChangeFileContent(path, resourceStream);
            }
        }

        public async Task GetAvailableCliVersions()
        {
            await CommandLineParser.Parse("get", "update-versions");
        }

        public void RemoveApplicationFile(string fileName)
        {
            fileSystemAbstraction.RemoveApplicationFile(fileName);
        }

        public void CheckcppAndhppExistInPath(string entityName, string path)
        {
            CheckEntityExistsInPath($"{entityName}.{Constants.ClassExtension}", path);
            CheckEntityExistsInPath($"{entityName}.{Constants.HeaderExtension}", path);
        }

        public void CheckEntityExistsInPath(string entityName, string path)
        {
            path = Path.Combine(path, entityName);
            fileSystemAbstraction.FindFile(ref path).ShouldBeTrue($"The {entityName} file is expected to exist in path {path}.");

        }

        public async Task UpdateCli(string file)
        {
            string path = Path.Combine(fileSystemAbstraction.CurrentDirectory.FullName, file);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Test.PlcNext.Deployment.{file}"))
            {
                fileSystemAbstraction.Load(stream, path);
            }

            await CommandLineParser.Parse("update", "cli", "-f", path);
        }

        public void CheckLibraryCreation(string commandArgsResourceKey = null)
        {
            string args = processManagerAbstraction.GetLastCommandArgs(Path.Combine(AssemblyDirectory, "bin", "LibraryBuilder.Core"));

            if (!string.IsNullOrEmpty(commandArgsResourceKey))
            {
                string commandArgsFile = args.Split('"', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Where(s => s.Contains(".txt")).FirstOrDefault();

                List<string> commandArgs = new List<string>();
                using (Stream fileStream = fileSystemAbstraction.GetDeletedFileStream(commandArgsFile))
                using (StreamReader fileReader = new StreamReader(fileStream))
                {
                    while (!fileReader.EndOfStream)
                    {
                        commandArgs.Add(fileReader.ReadLine().Trim());
                    }
                }
                commandArgs.RemoveAll(string.IsNullOrEmpty);
                string content = string.Join(Environment.NewLine, commandArgs);

                using (Stream expectedStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Test.PlcNext.Deployment.{commandArgsResourceKey}"))
                using (StreamReader expectedReader = new StreamReader(expectedStream))
                {
                    while (!expectedReader.EndOfStream)
                    {
                        string arg = expectedReader.ReadLine() ?? string.Empty;
                        arg = arg.Replace('\\', Path.DirectorySeparatorChar).Trim();
                        string escaped = Regex.Escape(arg ?? string.Empty).Replace("\\.\\*", ".*");
                        Regex regex = new Regex(escaped, RegexOptions.IgnoreCase);
                        int found = commandArgs.RemoveAll(regex.IsMatch);
                        found.ShouldBeGreaterThan(0, $"argument {arg} was expected in file but not found. Actual content:{Environment.NewLine}{content}");
                    }
                }

                commandArgs.ShouldBeEmpty("additional command args were not expected");
            }
        }

        public string ChangeLibraryBuilderLocation()
        {
            return fileSystemAbstraction.MoveApplicationFile("LibraryBuilder.Core", Path.Combine("subfolder", "LibraryBuilder.Core"));
        }

        public void RemoveLibraryBuilder()
        {
            fileSystemAbstraction.RemoveApplicationFile(Path.Combine("bin", "LibraryBuilder.Core"));
        }

        public async Task ChangeTarget(string target, string version, TargetChange change)
        {
            List<string> args = new List<string>(new[] { "set", "target", "--name", target, "--path", knownProjectName });

            if (!string.IsNullOrEmpty(version))
            {
                args.Add("--version");
                args.Add(version);
            }

            switch (change)
            {
                case TargetChange.Add:
                    args.Add("--add");
                    break;
                case TargetChange.Remove:
                    args.Add("--remove");
                    break;
                default:
                    break;
            }
            await CommandLineParser.Parse(args.ToArray());
        }

        public async Task InstallSdk(string sdk, string destination, bool force = false)
        {
            string path = Path.Combine(fileSystemAbstraction.CurrentDirectory.FullName, sdk);
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Test.PlcNext.Deployment.{sdk}"))
            {
                if (stream != null)
                    fileSystemAbstraction.Load(stream, path);
                else
                    fileSystemAbstraction.FileSystem.GetFile(sdk);
            }

            List<string> args = new List<string>(new[] {"install", "sdk", "--path", sdk, "--destination", destination});
            if (force)
            {
                args.Add("--force");
            }
            await CommandLineParser.Parse(args.ToArray());
        }

        public void CheckSdkInLocation(string location)
        {

            string path = Path.Combine(location, "DummySdk", "site-config-dummysdk-neon-pxc-linux-gnueabi");
            fileSystemAbstraction.FindFile(ref path).ShouldBeTrue
                ($"The site-config-dummysdk-neon-pxc-linux-gnueabi file is expected to exist in path {path}.");

            path = Path.Combine(location, "DummySdk", "version-dummysdk-neon-pxc-linux-gnueabi");
            fileSystemAbstraction.FindFile(ref path).ShouldBeTrue
                ($"The version-dummysdk-neon-pxc-linux-gnueabi file is expected to exist in path {path}.");

            path = Path.Combine(location, "DummySdk", "sysroots", "x86_64-dummysdk-mingw32", "usr", "site-config-dummysdk-neon-pxc-linux-gnueabi");
            fileSystemAbstraction.FindFile(ref path).ShouldBeTrue($"The site-config-dummysdk-neon-pxc-linux-gnueabi file is expected to exist in path {path}.");

        }

        public void CheckFileExists(string file, bool exists)
        {
            string searchedFile = file;
            fileSystemAbstraction.FindFile(ref searchedFile).ShouldBe(exists, $"File {file} was {(exists ? "expected" : "not expected")} to exist.");
        }

        public void CreateFile(string relativeFilePath)
        {
            string path = Path.Combine(fileSystemAbstraction.CurrentDirectory.FullName, relativeFilePath);
            fileSystemAbstraction.FileSystem.GetFile(path);
        }
        public void CreateDirectory(string relativeDirectoryPath)
        {
            string path = Path.Combine(fileSystemAbstraction.CurrentDirectory.FullName, relativeDirectoryPath);
            fileSystemAbstraction.FileSystem.GetDirectory(path);
        }


        public void SetCodeModel(string projectName, string[] externalLibs = null, string[] includePaths = null)
        {
            string content = string.Empty;

            using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Test.PlcNext.Deployment.DemoCodeModel.json"))
            using (StreamReader resourceReader = new StreamReader(resourceStream))
            {
                content = resourceReader.ReadToEnd();
            }
            content = content.Replace("{$name}", projectName);

            string libs = externalLibs?
                .Aggregate(string.Empty, (current, s) => current == string.Empty ? $"\\\"{s}\\\"" : $"{current} \\\"{s}\\\"")
                ?? string.Empty;
            string searchPaths = externalLibs?
                .Aggregate(string.Empty, (current, s) => $"{current}{Path.GetDirectoryName(s).Replace("\\", "\\\\")}:")
                                 ?? string.Empty;

            string includesLegacy = includePaths != null
                                  ?string.Join($",{Environment.NewLine}",
                                                               includePaths.Select(p => $"{{ \"path\" : \"{p}\" }}"))
                                  :string.Empty;
            string includes = includePaths != null
                ?"\"includes\" : [" + 
                 string.Join($",{Environment.NewLine}" , includePaths.Select(p => $"{{ \"path\" : \"{p}\" }}")) +
                 "]"
                :string.Empty;
            
            content = content.Replace("{$externalLibs}", libs);
            content = content.Replace("{$searchPaths}", searchPaths);
            content = content.Replace("{$includePaths}", includes);
            content = content.Replace("{$includePathsLegacy}", includesLegacy);

            string separator = string.Concat(Path.DirectorySeparatorChar).Replace("\\", "\\\\");
            content = content.Replace("/", separator);
            content = content.Replace("{$root}", fileSystemAbstraction.CurrentDirectory.FullName.Replace("\\", "\\\\"));
            JObject codeModel = JObject.Parse(content);
            cmakeConversationAbstraction.CodeModel = codeModel;
            
        }

        public void CreateCMakeBuildSystem(string[] targets, string type = "Debug")
        {
            foreach (string target in targets)
            {
                CreateFile(Path.Combine("intermediate", "cmake", target, type, "CMakeCache.txt"));
            }
        }

        public void WithOtherProgramInstance(int processId)
        {
            processManagerAbstraction.WithOtherProgramInstance(processId);
        }

        public async Task ExploreSdks()
        {
            await CommandLineParser.Parse("explore-sdks");
        }

        public async Task UpdateTargets(bool downgrade)
        {
            List<string> args = new List<string>(new[] { "update", "project-targets", "--path", knownProjectName });

            if (downgrade)
            {
                args.Add("--downgrade");
            }
            await CommandLineParser.Parse(args.ToArray());
        }

        public async Task Deploy(DeployCommandArgs deployArgs)
        {
            List<string> args = new List<string>
            {
                "deploy"
            };
            List<string> files = new List<string>();

            if (!string.IsNullOrEmpty(deployArgs.Id))
            {
                args.Add("--id");
                args.Add(deployArgs.Id);
            }

            if (!string.IsNullOrEmpty(deployArgs.LibraryVersion))
            {
                args.Add("--libraryversion");
                args.Add(deployArgs.LibraryVersion);
            }

            if (!string.IsNullOrEmpty(deployArgs.LibraryDescription))
            {
                args.Add("--librarydescription");
                args.Add(deployArgs.LibraryDescription);
            }

            if (!string.IsNullOrEmpty(deployArgs.EngineerVersion))
            {
                args.Add("--engineerversion");
                args.Add(deployArgs.EngineerVersion);
            }

            if (!string.IsNullOrEmpty(deployArgs.SolutionVersion))
            {
                args.Add("--solutionversion");
                args.Add(deployArgs.SolutionVersion);
            }

            if(deployArgs.LibraryInfo != null && deployArgs.LibraryInfo.Count() > 0)
            {
                args.Add("--libraryinfo");
                args.Add(string.Join("|", deployArgs.LibraryInfo));
            }

            if (!string.IsNullOrEmpty(deployArgs.LibraryLocation))
            {
                VirtualDirectory directory = fileSystemAbstraction.FileSystem.GetDirectory(deployArgs.LibraryLocation);
                foreach (VirtualFile file in directory.Files(searchRecursive:true))
                {
                    string destinationFolder = Path.GetDirectoryName(file.GetRelativePath(directory));
                    if (string.IsNullOrEmpty(destinationFolder))
                    {
                        destinationFolder = ".";
                    }
                    files.Add($"{file.FullName}|{destinationFolder}");
                }
            }

            if (!string.IsNullOrEmpty(deployArgs.MetaFileDirectory))
            {
                if (!string.IsNullOrEmpty(deployArgs.MetaFileOutputDirectory))
                {
                    files.Add($"{deployArgs.MetaFileDirectory}/*|{deployArgs.MetaFileOutputDirectory}");
                }
                else
                {
                    files.Add($"{deployArgs.MetaFileDirectory}/*|.");
                }
            }

            if (!string.IsNullOrEmpty(deployArgs.OutputDirectory))
            {
                args.Add("--output");
                args.Add(deployArgs.OutputDirectory);
            }

            if (!string.IsNullOrEmpty(deployArgs.BuildType))
            {
                args.Add("--buildtype");
                args.Add(deployArgs.BuildType);
            }

            if (deployArgs.Sign == true)
            {
                args.Add("--sign");
            }

            if (!string.IsNullOrEmpty(deployArgs.PKCS12))
            {
                args.Add("--pkcs12");
                args.Add(deployArgs.PKCS12);
            }

            if (!string.IsNullOrEmpty(deployArgs.PrivateKey))
            {
                args.Add("--privatekey");
                args.Add(deployArgs.PrivateKey);
            }

            if (!string.IsNullOrEmpty(deployArgs.PublicKey))
            {
                args.Add("--publickey");
                args.Add(deployArgs.PublicKey);
            }

            if (deployArgs.Certificates != null && deployArgs.Certificates.Any())
            {
                args.Add("--certificates");
                args.Add(string.Join(",", deployArgs.Certificates));
            }

            if (deployArgs.Password != null)
            {
                args.Add("--password");
                args.Add(deployArgs.Password);
            }

            if (deployArgs.Timestamp)
            {
                args.Add("--timestamp");
            }

            if (deployArgs.NoTimestamp)
            {
                args.Add("--notimestamp");
            }

            if (!string.IsNullOrEmpty(deployArgs.TimestampConfiguration))
            {
                args.Add("--timestampconfiguration");
                args.Add(deployArgs.TimestampConfiguration);
            }

            if (deployArgs.Targets != null && deployArgs.Targets.Any())
            {
                args.Add("-t");
                foreach (string target in deployArgs.Targets)
                {
                    args.Add(target);
                }
            }

            if (deployArgs.ExternalLibraries != null && deployArgs.ExternalLibraries.Any())
            {
                foreach (string lib in deployArgs.ExternalLibraries)
                {
                    files.Add(FormatExternalLibraryFilesConvert(lib));
                }
            }

            if (deployArgs.ExcludedFiles != null && deployArgs.ExcludedFiles.Any())
            {
                args.Add("--excludefiles");
                foreach (string file in deployArgs.ExcludedFiles)
                {
                    args.Add(file);
                }
            }

            if ((deployArgs.Files != null && deployArgs.Files.Any()) ||
                files.Any())
            {
                args.Add("-f");
                foreach (string file in files.Concat(deployArgs.Files??Enumerable.Empty<string>()))
                {
                    args.Add(file);
                }
            }

            await CommandLineParser.Parse(args.ToArray());

            string FormatExternalLibraryFilesConvert(string lib)
            {
                int indexOfQuotes = lib.IndexOf('\"');
                int lastIndex = indexOfQuotes < 0
                                    ? lib.LastIndexOf(',')
                                    : lib.LastIndexOf(',', 0, indexOfQuotes);
                if (lastIndex >= 0)
                {
                    return $"{lib.Substring(lastIndex + 1)}|.|{lib.Substring(0, lastIndex)}";
                }

                return $"{lib}|.";
            }
        }

        public void CheckFilesExistInLocation(Dictionary<string, string> filesAndContent)
        {
            foreach (KeyValuePair<string, string> entry in filesAndContent)
            {
                string path = GetPathOfFile(entry.Key);
                if (entry.Value == null)
                {
                    fileSystemAbstraction.FileExists(entry.Key).ShouldBeTrue($"file {entry.Key} was expected to exist.");
                    continue;
                }
                using (Stream fileStream = fileSystemAbstraction.Open(path))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string content = reader.ReadToEnd();
                    content.Contains(entry.Value).ShouldBeTrue($"Content '{entry.Value}' was expected to exist." +
                        $" Actual content{Environment.NewLine}{content}");

                }
            }
        }

        public void CheckNoDatatypeWorksheetGenerated()
        {
            knownProjectName.ShouldNotBeNullOrEmpty("Cannot check if project name is not known.");
            string path = GetPathOfGeneratedFile($"{knownProjectName}DataTypes.{Constants.DatatypeWorksheetExtension}", 
                                                 false, Constants.MetadataFolderName);
        }

        public void CheckCmakeArgumentsUsed(IEnumerable<string> expectedArgs)
        {
            string cmakeArg = processManagerAbstraction.GetLastCommandArgs("cmake");

            IEnumerable<string> cmakeArgs = cmakeArg.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            //cmakeArgs.ShouldBeSubsetOf(expectedArgs);
            expectedArgs.ShouldBeSubsetOf(cmakeArgs);
        }

        public void FindTargetOnExplore(string name, string version)
        {
            sdkExplorerAbstraction.FindTargetOnExplore(name, version);
        }
    }
}
