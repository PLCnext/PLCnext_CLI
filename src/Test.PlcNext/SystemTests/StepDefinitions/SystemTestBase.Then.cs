#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using LightBDD.Framework;
using LightBDD.XUnit2;
using Test.PlcNext.Tools;
using PlcNext.Common.Tools;
using System.IO;
using PlcNext.Common.Build;
using PlcNext.Common.Commands;
using PlcNext.Common.Installation;
using PlcNext.Common.Project;
using PlcNext.Common.Tools.IO;
using PlcNext.Common.Tools.Security;
using PlcNext.Common.Tools.Settings;
using PlcNext.Common.Tools.Web;
using Xunit.Sdk;
using System.Threading.Tasks;
using PlcNext.Common.Templates;
using PlcNext.Common.Tools.SDK;

namespace Test.PlcNext.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase
    {
        protected void Then_the_project_NAME_was_created(string name)
        {
            ScenarioContext.CheckProjectCreated(name);
        }

        protected void Then_the_project_NAME_was_again_created(string name)
        {
            ScenarioContext.CheckProjectCreated(name);
            ScenarioContext.CheckProjectCreatedTwice();
        }

        protected void Then_the_project_NAME_was_created_inside_root_folder(string name)
        {
            ScenarioContext.CheckProjectCreatedInFolder(name, null);
        }

        protected void Then_the_project_NAME_was_created_in_folder(string name, string folder)
        {
            ScenarioContext.CheckProjectCreatedInFolder(name, folder);
        }

        protected void Then_the_user_was_informed_that_the_artifact_exists_already()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(FileExistsException));
        }

        protected async Task Then_the_setting_has_the_value(string setting, string value)
        {
            await ScenarioContext.CheckSetting(setting, value);
        }

        protected async Task Then_the_setting_has_the_values(string setting, params string[] values)
        {
            await ScenarioContext.CheckSetting(setting, values);
        }

        protected void Then_the_user_was_informed_that_the_setting_does_not_exist()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SettingNotFoundException));
        }

        protected void Then_the_user_was_informed_that_the_setting_is_no_collection()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SettingIsNotCollectionException));
        }

        protected void Then_the_user_was_informed_that_a_value_is_necessary()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SettingValueIsEmptyException));
        }

        protected void
            Then_the_user_was_informed_that_the_value_does_not_exist_and_asked_if_the_value_SUGGESTION_was_suggested(
                string suggestion)
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SettingValueNotFoundException));
        }

        protected void Then_the_user_was_informed_that_only_bool_values_are_allowed()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(BoolSettingsValueFormatException));
        }

        protected void Then_the_user_was_informed_that_there_are_multiple_root_namespaces()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(MultipleRootNamespacesException));
        }

        protected void Then_the_project_supports_the_targets(params string[] targets)
        {
            ScenarioContext.CheckTargetSupported(targets);
        }

        protected void Then_the_project_contains_component_and_program_cpp_and_hpp_files()
        {
            ScenarioContext.CheckCppAndHppFilesExist();
        }

        protected void Then_the_project_contains_a_component_with_name(string name)
        {
            ScenarioContext.CheckComponentHasName(name);
        }

        protected void Then_the_project_contains_an_acfcomponent_with_name(string name)
        {
            ScenarioContext.CheckIsAcfComponent(name);
            ScenarioContext.CheckComponentHasName(name);
        }

        protected void Then_the_project_contains_a_program_with_name(string name)
        {
            ScenarioContext.CheckProgramHasName(name);
        }
        
        protected void Then_no_files_were_changed_or_added_to_the_workspace()
        {
            ScenarioContext.FileAddedOrChanged(0);
        }

        protected void Then_only_the_cmake_file_was_added_to_the_workspace()
        {
            ScenarioContext.FileAddedOrChanged(1);
        }


        protected void Then_existing_files_were_not_deleted()
        {
            ScenarioContext.ExistingFilesDeleted(0);
        }

        protected void Then_the_typemeta_file_contains_the_following_structure(
            params TypemetaStructure[] typemetaStructures)
        {
            ScenarioContext.CheckTypemetaFile(typemetaStructures);
        }

        protected void Then_the_error_is_shown(ErrorInformation error)
        {
            ScenarioContext.CheckForError(error);
        }

        protected void Then_the_user_was_informed_that_the_target_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(TargetNameNotFoundException));
        }

        protected void Then_the_user_was_informed_that_the_project_does_not_contain_any_target()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(NoAssingedTargetsException));
        }

        protected void Then_the_user_was_informed_that_the_component_option_is_missing()
        {
            ScenarioContext.CheckUserInformed("Required option 'c, component' is missing.",
                                              "message of wrong option specification expected");
        }

        protected void Then_the_user_was_informed_that_the_artifact_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(RelationshipTypeNotFoundException));
        }

        protected void Then_the_build_was_executed()
        {
            ScenarioContext.CheckBuildStartedAndEnded();
        }

        protected void Then_the_code_entity_was_created()
        {
            ScenarioContext.CheckCodeEntityCreated();
        }

        protected void Then_codefiles_were_generated_for_the_component(string component, string[] ports)
        {
            ScenarioContext.CheckGeneratedComponentCodeFiles(component, ports);
        }

        protected void Then_codefiles_were_not_generated_for_the_component(string component)
        {
            ScenarioContext.CheckGeneratedComponentCodeFiles(component, shouldExist: false);
        }

        protected void Then_the_library_files_are_generated()
        {
            ScenarioContext.CheckLibraryIsGenerated();
        }

        protected void Then_the_provider_files_are_generated_for_component(string component, string[] programs)
        {
            ScenarioContext.CheckProviderIsGeneratedForComponent(component, programs);
        }

        protected void Then_there_are_progmeta_files_with_the_following_content(params ProgmetaData[] progmetaDatas)
        {
            ScenarioContext.CheckProgmetaFiles(progmetaDatas);
        }

        protected void Then_there_are_compmeta_files_with_the_following_content(params CompmetaData[] compmetaDatas)
        {
            ScenarioContext.CheckCompmetaFiles(compmetaDatas);
        }

        protected void Then_the_libmeta_file_is_generated_with_the_components(params string[] components)
        {
            ScenarioContext.CheckGeneratedLibmeta(components);
        }

        protected void Then_the_acfconfig_file_is_generated_with_the_components(string ns, string componentname)
        {
            ScenarioContext.CheckGeneratedAcfConfig(ns, componentname);
        }

        protected void Then_the_components_namespace_starts_with_namespace(string entityName, string ns)
        {
            ScenarioContext.CheckSourceEntitiesNamespace(ns, entityName);
        }

        protected void Then_the_programs_namespace_starts_with_namespace(string entityName, string ns)
        {
            ScenarioContext.CheckSourceEntitiesNamespace(ns, entityName);
        }

        protected void Then_the_generated_components_namespace_starts_with_namespace(
            string entityName, string ns)
        {
            ScenarioContext.CheckGeneratedComponentsNamespace(ns, entityName);
        }

        protected void Then_the_user_was_informed_that_the_component_was_created_successfully()
        {
            ScenarioContext.CheckUserInformed("Successfully created template 'component'"
                                            , "an information that the component was created was expected");
        }

        protected void Then_the_user_was_informed_that_the_program_was_created_successfully()
        {
            ScenarioContext.CheckUserInformed("Successfully created template 'program'"
                                            , "an information that the program was created was expected");
        }


        protected void Then_the_typemeta_method_looks_like_NAME(string name)
        {
            ScenarioContext.CheckTypemetaMethod(name);
        }

        protected void Then_the_version_of_the_CLI_is_VERSION(Version version)
        {
            ScenarioContext.CheckNewCliVersion(version);
        }

        protected void Then_the_new_CLI_setting_has_the_value(string setting, string value)
        {
            ScenarioContext.CheckNewSettingWasChanged(setting, value);
        }

        protected void Then_the_user_was_informed_that_the_setting_was_lost(string setting)
        {
            ScenarioContext.CheckUserInformed("settings could not be migrated", "migration error expected");
            ScenarioContext.CheckUserInformed(setting, "setting in migration message expected");
        }

        protected void Then_the_user_was_informed_that_the_site_is_not_reachable(string site)
        {
            ScenarioContext.CheckUserInformedOfError(typeof(DownloadFileNotReachableException));
        }

        protected void Then_the_user_was_informed_that_the_repository_file_was_manipulated()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SignatureValidationException));
        }

        protected void Then_the_user_was_informed_that_the_archive_file_was_manipulated()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(HashValidationException));
        }

        protected void Then_the_user_was_informed_that_the_version_is_not_available(Version version)
        {
            ScenarioContext.CheckUserInformedOfError(typeof(UnkownVersionException));
        }

        protected void Then_the_user_was_informed_that_the_CLI_is_up_to_date()
        {
            ScenarioContext.CheckUserInformed($"Version is up-to-date", "version unavailable message expected.");
        }

        protected void Then_the_user_was_informed_that_the_versions_are_available(params Version[] versions)
        {
            ScenarioContext.CheckUserInformed($"versions are available", "versions available should be shown.");

            foreach (Version version in versions)
            {
                ScenarioContext.CheckUserInformed(version.ToString(3), $"specified version was expected");
            }
        }

        protected void Then_the_user_was_informed_that_the_file_extension_is_unkown()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(UnsupportedArchiveFormatException));
        }

        protected void Then_the_user_was_informed_that_the_cli_public_key_is_missing()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(PublicKeyFileNotFoundException));
        }

        protected void Then_the_cmake_file_was_generated()
        {
            ScenarioContext.CheckCMakeFile();
        }

        protected void Then_the_component_exists_in_path(string name, string path)
        {
            ScenarioContext.CheckcppAndhppExistInPath(name, path);
        }

        protected void Then_the_program_exists_in_path(string name, string path)
        {
            ScenarioContext.CheckcppAndhppExistInPath(name, path);
        }

        protected void Then_the_entity_was_created_in_default_namespace(string entityName)
        {
            ScenarioContext.CheckCodeEntityCreatedInDefaultNamespace($"{entityName}.cpp");
            ScenarioContext.CheckCodeEntityCreatedInDefaultNamespace($"{entityName}.hpp");
        }

        protected void Then_the_command_was_executed_without_error()
        {
            ScenarioContext.CheckUserInformed("generated all", "message of successfull generation expected");
        }

        protected void Then_the_user_was_informed_that_the_comment_is_ambiguous()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(AmbiguousRelationshipTypeException));
        }

        protected void Then_the_user_was_informed_that_the_comment_points_to_nonexisting_component()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(WildEntityException));
        }

        protected void Then_the_user_was_informed_that_the_component_name_is_ambiguous()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(AmbiguousRelationshipTypeException));
        }

        protected void Then_the_user_was_informed_that_the_library_id_is_malformatted()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(LibraryIdMalformattedException));
        }

        protected void Then_the_library_was_generated_with_the_following_content(string content)
        {
            ScenarioContext.CheckLibraryCreation(content);
        }

        protected void Then_the_library_was_generated()
        {
            ScenarioContext.CheckLibraryCreation();
        }


        protected void Then_the_user_was_informed_that_the_library_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(LibraryNotFoundException));
        }

        protected void Then_the_user_was_informed_that_the_meta_files_were_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(MetaLibraryNotFoundException));
        }

        protected void Then_the_user_was_informed_that_a_compmeta_file_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(MetaFileNotFoundException));
        }

        protected void Then_the_user_was_informed_that_the_library_builder_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(LibraryBuilderNotFoundException));
        }

        protected void Then_the_user_was_informed_that_a_target_can_not_be_set_for_this_project()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(TargetNotSettableForProjectException));
        }

        protected void Then_the_user_was_informed_that_an_option_must_be_provided()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(SetTargetsOptionMissingException));
        }

        protected void Then_the_user_was_informed_that_the_target_is_already_supported()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(TargetAlreadySupportedException));
        }

        protected void Then_the_sdk_is_available_in_DESTINATION(string location)
        {
            ScenarioContext.CheckSdkInLocation(location);
        }

        protected void Then_the_user_was_informed_that_the_library_option_is_malformatted()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(MalformedExternalLibrariesOptionException));
        }

        protected void Then_the_user_was_informed_that_the_target_is_ambiguous()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(TargetNameAmbiguousException));
        }

        protected void Then_the_user_was_informed_that_the_library_option_is_wrong_combined()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(WrongCombinedExternalLibrariesException));
        }

        protected void Then_the_user_was_informed_that_the_cmake_build_system_was_not_found()
        {
            ScenarioContext.CheckUserInformedOfWarning("Could not find cmake build system",
                                                       "message of missing cmake build system expected");
        }

        protected void Then_the_user_was_informed_that_the_target_can_not_be_updated()
        {
            ScenarioContext.CheckUserInformedOfError(typeof(NoHigherTargetAvailableException));
        }
    }
}
