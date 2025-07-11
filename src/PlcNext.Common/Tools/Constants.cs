﻿#region Copyright
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
using System.Reflection;
using System.Text;

namespace PlcNext.Common.Tools
{
    public static class Constants
    {
        //folder names
        public const string SourceFolderName = "src";
        public const string IntermediateFolderName = "intermediate";
        public const string MetadataFolderName = "config";
        public const string GeneratedCodeFolderName = "code";
        public const string CmakeFolderName = "cmake";
        public const string CmakeAutoBuildFolderName = "cmakeAutoBuild";
        public const string ReleaseFolderName = "Release";
        public const string DebugFolderName = "Debug";
        public const string LibraryFolderName = "bin";
        public const string CSharpProjectItemsFolderName = "ProjectItems";
        public const string CSharpHelpFolderName = "Help";
        public const string ConfigIndependentFiles = "Common";
        public const string DeployHelpDirectoryName = "hlp";

        //extensions
        public const string TypemetaExtension = "typemeta";
        public const string LibmetaExtension = "libmeta";
        public const string CompmetaExtension = "compmeta";
        public const string ProgmetaExtension = "progmeta";
        public const string SharedObjectExtension = "so";
        public const string EngineeringLibraryExtension = "pcwlx";
        public const string HeaderExtension = "hpp";
        public const string ClassExtension = "cpp";
        public const string ScriptFileExtensionWin = "bat";
        public const string ScriptFileExtensionLinux = "sh";
        public const string DatatypeWorksheetExtension = "dt";
        public const string AcfConfigExtension = ".acf.config";

        //other
        public const string Tab = "    ";
        public const int StreamCopyBufferSize = 4096;
        public const int CMakeServerTimeout = 6000;
        public const string OutputArgumentName = "output";
        public const string NoIncludePathDetection = "no-include-path-detection";
        public const string MetaPathArgumentName = "metapath";
        public const string LibraryLocationArgumentName = "compilationpath";
        public const string IdArgumentName = "id";
        public const string TargetArgumentName = "target";
        public const string ExternalLibrariesArgumentName = "externallibraries";
        public const string FilesArgumentName = "files";
        public const string BuildTypeArgumentName = "buildtype";
        public const int MaxIORetries = 10;
        public const char OptionsSeparator = ',';
        public const string DescriptionArgumentKey = "librarydescription";
        public const string VersionArgumentKey = "libraryversion";
        public const string EngineerVersionArgumentKey = "engineerversion";
        public const string SolutionVersionArgumentKey = "solutionversion";
        public const string LibraryInfoArgumentKey = "libraryinfo";
        public const string SignArgumentKey = "sign";
        public const string pkcs12ArgumentKey = "pkcs12";
        public const string privateKeyArgumentKey = "privatekey";
        public const string publicKeyArgumentKey = "publickey";
        public const string certificatesArgumentKey = "certificates";
        public const string timestampArgumentKey = "timestamp";
        public const string noTimestampArgumentKey = "notimestamp";
        public const string passwordArgumentKey = "password";
        public const string timestampConfiguration = "timestampconfiguration";

        //file names
        public const string PublicKeyFileName = "public_cli_repository_key.xml";
        public const string ProjectFileName = "plcnext.proj";
        public const string FileNamesFile = "file-names.xml";
        public const string EnvironmentSetupScriptFile = "EnvironmentSetup";
        public const string CMakeFileName = "CMakeLists.txt";
        public const string CMakeCommandArgsFileName = "CMakeFlags.txt";
        public const string CMakeTimestampFileName = "FlagsTimestamp.txt";
        public const string ConfigFileName = "PLCnextSettings.xml";
        public const string MSBuildName = "msbuild";
        public const string DotNetName = "dotnet";

        //progress visualizer
        public const int ProgressMaxResolution = 100;
        public const int ProgressUpdateInterval = 10;
        public const int ConsoleInfiniteProgressDotIntervalInMs = 1000;

        //settings
        public const string SdkPathsKey = "SdkPaths";
        public const string AttributePrefixKey = "AttributePrefix";
        public const string LogFilePathKey = "LogFilePath";
        public const string UseSystemCommandsKey = "UseSystemCommands";
        public const char SettingSplitChar = ';';

        //library builder
        public const string CommandFileOption = "/cmd";
        public const string OutputOption = "/out";
        public const string GuidOption = "/id";
        //0: File Type
        //1: File Source Path
        //2: Guid
        //3: Destination Path Relative to Logical Elements Empty if root
        public const string FileOptionPattern = "/file \"{0}:{1},Id={2}:{3}\"";
        //0: Directory path
        //1: Folder type
        //2: Guid
        public const string DirectoryOptionPattern = "/folder \"{0},FolderType={1},Id={2}\"";
        //0: Target full name (foldername)
        //1: Target name
        //2: Target version
        //3: Guid
        //4: Target name,version
        public const string PlcnextNativeLibraryOptionPattern =
            "/plcnextnativelibfolder \"{0},FolderName={4},DeviceProfileType={1},DeviceProfileVersion={2},Id={3}\"";
        public const string EclrNativeLibraryOptionPattern =
            "/eclrnativelibfolder \"{0},AssemblyName={5},FolderName={4},DeviceProfileType={1},DeviceProfileVersion={2},Id={3}\"";
        public const string LibmetaFileType = "MetaLibrary";
        public const string TypemetaFileType = "MetaTypes";
        public const string ProgmetaFileType = "MetaProgram";
        public const string CompmetaFileType = "MetaComponent";
        public const string ComponentFolderType = "MetaComponentFolder";
        public const string ProgramFolderType = "MetaProgramFolder";
        public const string DataTypeWorksheetType = "DataTypeWorksheet";
        public const string AcfConfigurationType = "AcfConfiguration";
        public const string PrecompiledLibraryType = "PrecompiledLibrary";
        //0: Property key=value
        public const string KeyOptionPattern = "/key \"{0}={1}\"";
        public const string LibraryVersionKey = "LibraryVersion";
        public const string LibraryDescriptionKey = "LibraryDescription";
        public const string EngineerVersionOptionPattern = "/pnever {0}";
        public const string SolutionVersionPattern = "/ver {0}";
        public const string LibraryInfoPattern = " /key \"LibraryInfo:{0}={1}\"";

        //signing arguments library builder
        public const string CertificateContainerPattern = "/seccertificatecontainer {0}";
        public const string TimestampServerPattern = "/sectimestamp {0}";
        public const string TimestampConfigurationPattern = "/sectimestampconfiguration {0}";
        public const string PublicKeyPattern = "/secpublickey {0}";
        public const string PrivateKeyPattern = "/secprivatekey {0}";
        public const string PasswordPattern = "/secpw {0}";
        //0: list of filepaths, separated by ','
        public const string CertificatesPattern = "/seccertificates {0}";


        //Output formatter
        public const string CommandKey = "command";
        public const string CommandArgumentsKey = "command-arguments";
        public const string MessageFormat = "message-format";
    }
}
