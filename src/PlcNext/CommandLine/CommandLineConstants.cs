#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace PlcNext.CommandLine
{
    public static class CommandLineConstants
    {
        public const string PathOption = "path";
        public const char PathChar = 'p';

        public const string AllOption = "all";
        public const char AllChar = 'a';

        //BuildVerb
        public const string BuildVerb = "build";
        public const string BuildtypeOption = "buildtype";
        public const char BuildtypeChar = 'b';
        public const string ConfigureOption = "configure";
        public const string NoconfigureOption = "noconfigure";
        public const string OutputOption = "output";
        public const char OutputChar = 'o';
        public const string TargetOption = "target";
        public const char TargetChar = 't';

        //GenerateVerb
        public const string GenerateVerb = "generate";

        //GetIncludePathsVerb
        public const string IncludepathsVerb = "include-paths";

        //GetCompilerSpecifications
        public const string CompilerSpecificationsVerb = "compiler-specifications";

        //GetSdksVerb
        public const string SdksVerb = "sdks";
        public const string ProjectSdksVerb = "project-sdks";

        //GetSettingsVerb //SetSettingVerb
        public const string SettingVerb = "setting";

        //GetTargetsVerb
        public const string ShortOption = "short";
        public const char ShortChar = 's';

        //GetVerb
        public const string GetVerb = "get";

        //InstallSdkVerb
        public const string SdkVerb = "sdk";
        public const string DestinationOption = "destination";
        public const char DestinationChar = 'd';
        public const string ForceOption = "force";
        public const char ForceChar = 'f';

        //InstallVerb
        public const string InstallVerb = "install";

        //SetSettingsVerb
        public const string AddOption = "add";

        //SetSettingsVerb //SetTargetsVerb
        public const string RemoveOption = "remove";
        public const string ClearOption = "clear";

        //SetTargetsVerb
        public const string TargetVerb = "target";
        public const string NameOption = "name";
        public const char NameChar = 'n';
        public const string VersionOption = "version";
        public const char VersionChar = 'v';
        public const char AddChar = 'a';
        public const char RemoveChar = 'r';

        //SetVerb
        public const string SetVerb = "set";

        //UpdateTargetsVerb //GetTargetsVerb
        public const string TargetsVerb = "targets";
        public const string ProjectTargetsVerb = "project-targets";

        //GetProjectName // GetProjectNamespace
        public const string ProjectNameVerb = "project-name";
        public const string ProjectNamespaceVerb = "project-namespace";

        //UpdateVerb
        public const string UpdateVerb = "update";

        //MigrateCliVerb
        public const string MigrateCliVerb = "migrate-old-cli";

        //GetProjectInformationVerb
        public const string ProjectInformationVerb = "project-information";

        //UpdateTargets
        public const string DowngradeOption = "downgrade";
        public const char DowngradeChar = 'd';

        //DeployVerb
        public const string DeployVerb = "deploy";
        
        //Settings description
        public const char DescriptionChar = 'd';
        public const string DescriptionOption = "description";
    }

}
