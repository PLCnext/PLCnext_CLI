#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.Features;
using Test.PlcNext.SystemTests.Tools;
using Test.PlcNext.Tools;

namespace Test.PlcNext.SystemTests.StepDefinitions
{
	public abstract partial class SystemTestBase
	{
	    protected async Task When_I_create_a_new_project_with_name(string name)
	    {
	        await ScenarioContext.CreateProject(name);
	    }

        protected async Task When_I_create_a_new_project_with_componentname(string componentname)
        {
            await ScenarioContext.CreateProject(componentName: componentname);
        }

        protected async Task When_I_create_a_new_acfproject_with_componentname(string componentName)
        {
            await ScenarioContext.CreateProject("Project", componentName: componentName, acfproject: true);
        }
        protected async Task When_I_create_a_new_project_with_name_and_componentname(string name, string componentname)
        {
            await ScenarioContext.CreateProject(name, componentname);
        }

        protected async Task When_I_create_a_new_project_with_programname(string programname)
        {
            await ScenarioContext.CreateProject(programName: programname);
        }

        protected async Task When_I_create_a_new_project()
	    {
            await ScenarioContext.CreateProject();
        }

	    protected async Task When_I_create_a_new_project_with_name_in_folder(string name, string folder)
	    {
            await ScenarioContext.CreateProject(projectName: name, folder: folder);
        }

	    protected async Task When_I_create_a_new_project_with_name_forced(string name)
	    {
            await ScenarioContext.CreateProject(projectName: name, forced: true);
        }

        protected async Task When_I_create_a_new_component_with_name(string name, bool forced)
        {
            await ScenarioContext.CreateComponent(forced, false, name);
        }

        protected async Task When_I_create_a_new_component_with_path(string path, bool addPath, bool forced)
        {
            await ScenarioContext.CreateComponent(forced, addPath, path: path);
        }

        protected async Task When_I_create_a_new_component(bool forced, bool addPath)
        {
            await ScenarioContext.CreateComponent(forced, addPath);
        }

	    protected async Task When_I_change_the_setting_to_the_value(string setting, string value)
	    {
	        await ScenarioContext.ChangeSetting(setting, value, SystemTestContext.SettingChange.Change);
        }

	    protected async Task When_I_add_the_value_to_the_setting_collection(string collection, string value)
	    {
	        await ScenarioContext.ChangeSetting(collection, value, SystemTestContext.SettingChange.Add);
        }

	    protected async Task When_I_remove_the_value_from_the_setting_collection(string collection, string value)
	    {
	        await ScenarioContext.ChangeSetting(collection, value, SystemTestContext.SettingChange.Remove);
        }

	    protected async Task When_I_clear_the_setting(string setting)
	    {
	        await ScenarioContext.ChangeSetting(setting, null, SystemTestContext.SettingChange.Clear);
        }

	    protected async Task When_I_clear_the_setting_with_the_value(string setting, string value)
	    {
	        await ScenarioContext.ChangeSetting(setting, value, SystemTestContext.SettingChange.Clear);
        }

	    protected async Task When_I_change_the_setting_without_any_value(string setting)
	    {
	        await ScenarioContext.ChangeSetting(setting, null, SystemTestContext.SettingChange.Change);
        }
	    protected async Task When_I_add_without_any_value_to_the_setting_collection(string collection)
	    {
	        await ScenarioContext.ChangeSetting(collection, null, SystemTestContext.SettingChange.Add);
        }

	    protected async Task When_I_remove_without_any_value_from_the_setting_collection(string collection)
	    {
	        await ScenarioContext.ChangeSetting(collection, null, SystemTestContext.SettingChange.Remove);
        }

        protected async Task When_I_create_a_new_component_with_name_for_project_in_specific_folder(string name, bool forced)
        {
            await ScenarioContext.CreateComponent(forced, true, name);
        }

        protected async Task When_I_create_a_new_program_with_name_for_component(string name, string component, bool forced)
        {
            await ScenarioContext.CreateProgram(forced, false, name, component);
        }

        protected async Task When_I_create_a_new_program(bool forced)
        {
            await ScenarioContext.CreateProgram(forced, true);
        }
        protected async Task When_I_create_a_new_program_with_name_for_component_for_project_in_specific_folder(string name, string component, bool forced)
        {
            await ScenarioContext.CreateProgram(forced, true, name, component);
        }

        protected async Task When_I_create_a_new_program_for_component(string component, bool forced, bool addPath)
        {
            await ScenarioContext.CreateProgram(forced, addPath, null, component);
        }

        protected async Task When_I_create_a_new_program_with_name_for_component_with_source_folders(
            string name, string component, params string[] sourceFolders)
        {
            await ScenarioContext.CreateProgram(false, false, name, component, sourceFolders:sourceFolders);
        }

        protected async Task When_I_create_a_new_program_with_path_for_component(string path, bool addPath, bool forced, string componentName)
        {
            await ScenarioContext.CreateProgram(forced, addPath, null, componentName, path);
        }

        protected async Task When_I_generate_all_metafiles()
	    {
	        await ScenarioContext.GenerateMeta(true);
	    }

        protected async Task When_I_generate_all_metafiles_without_auto_detection()
	    {
	        await ScenarioContext.GenerateMeta(true, autoDetection:false);
	    }

	    protected async Task When_I_generate_all_metafiles_with_the_source_directories(params string[] directories)
	    {
	        await ScenarioContext.GenerateMeta(true, directories);
        }

        protected async Task When_I_generate_all_metafiles_with_includes(params string[] includes)
        {
            await ScenarioContext.GenerateMeta(true, includes: includes);
        }

        protected async Task When_I_generate_all_codefiles()
        {
            await ScenarioContext.GenerateCode(true);
        }

        protected async Task When_I_generate_all_codefiles_without_auto_detection()
        {
            await ScenarioContext.GenerateCode(true, autoDetection:false);
        }
        protected async Task When_I_generate_all_codefiles_with_includes(params string[] includes)
        {
            await ScenarioContext.GenerateCode(true, includes:includes);
        }
        protected async Task When_I_generate_all_codefiles_from_inside_the_project_folder()
        {
            await ScenarioContext.GenerateCode(false);
        }

	    protected async Task When_I_generate_all_codefiles_with_the_source_directories(params string[] directories)
	    {
	        await ScenarioContext.GenerateCode(false, directories);
        }

        protected void When_the_file_system_throws_an_error_on_accessing_the_file(string path)
	    {
	        ScenarioContext.ThrowOnFileAccess(path);
        }

        protected void When_the_processManager_starts_a_process_that_throws_an_error()
        {
            ScenarioContext.StartProcessThrowsError();
        }

	    protected async Task When_I_generate_all_files_from_inside_the_project_folder()
	    {
	        await ScenarioContext.GenerateMeta(false);
        }

        protected async Task When_I_build_the_project()
        {
            await ScenarioContext.Build(true);
        }

        protected async Task When_I_execute_build_from_inside_the_project_folder()
        {
            await ScenarioContext.Build(false);
        }

        protected async Task When_I_build_the_project_for_target(string target)
        {
            await ScenarioContext.BuildForTarget(true, target);
        }

        protected async Task When_I_build_the_project_from_inside_the_project_folder_for_target(string target, string version)
        {
            await ScenarioContext.BuildForTarget(false, target, version);
        }

        protected async Task When_I_build_the_project_with_build_type(string buildType)
        {
            await ScenarioContext.BuildWithBuildType(buildType);
        }

	    protected async Task When_I_update_the_CLI_to_the_latest_version()
	    {
	        await ScenarioContext.UpdateCli();
	    }

	    protected async Task When_I_update_the_CLI_to_the_latest_version_with_the_proxy(string proxy)
	    {
	        await ScenarioContext.UpdateCli(null, proxy);
        }

        protected async Task When_I_update_the_CLI_to_the_version(Version version)
	    {
	        await ScenarioContext.UpdateCli(version);
        }

	    protected async Task When_I_get_all_available_CLI_versions()
	    {
	        await ScenarioContext.GetAvailableCliVersions();
        }

	    protected async Task When_I_update_the_CLI_with_the_file(string file)
	    {
	        await ScenarioContext.UpdateCli(file);
        }

	    protected async Task When_I_generate_the_library()
	    {
	        await ScenarioContext.GenerateLibrary(null);
	    }

        protected async Task When_I_generate_the_library(LibraryCommandArgs libraryCommandArgs)
	    {
	        await ScenarioContext.GenerateLibrary(libraryCommandArgs);
        }

        protected async Task When_I_add_the_target(string target, string version)
        {
            await ScenarioContext.ChangeTarget(target, version, SystemTestContext.TargetChange.Add);
        }

        protected async Task When_I_remove_the_target(string target, string version)
        {
            await ScenarioContext.ChangeTarget(target, version, SystemTestContext.TargetChange.Remove);
        }

        protected async Task When_I_set_the_target(string target)
        {
            await ScenarioContext.ChangeTarget(target, null, SystemTestContext.TargetChange.None);
        }

        protected async Task When_I_install_SDK_to_DESTINATION(string sdk, string destination)
        {
            await ScenarioContext.InstallSdk(sdk, destination);
        }

        protected async Task When_I_update_the_project_targets()
        {
            await ScenarioContext.UpdateTargets(false);
        }

        protected async Task When_I_update_the_project_targets_with_downgrade_option()
        {
            await ScenarioContext.UpdateTargets(true);
        }

        protected async Task When_I_deploy(DeployCommandArgs args)
        {
            await ScenarioContext.Deploy(args);
        }
    }
}