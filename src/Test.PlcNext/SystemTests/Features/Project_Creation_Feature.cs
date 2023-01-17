#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.StepDefinitions;
using Xunit.Abstractions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
	[FeatureDescription(@"User creates new project via CLI with different parameters.")]
    [Label("ART-PLCNEXT-Toolchains-1585")]
	public class Project_Creation_Feature : MockedSystemTestBase
	{
		[Scenario]
		public async Task Create_new_project_with_specific_name()
		{
            await Runner.AddSteps(
				_ => Given_is_an_empty_workspace(),
				_ => When_I_create_a_new_project_with_name("NewProject"),
				_ => Then_the_project_NAME_was_created("NewProject")).RunAsyncWithTimeout();
	    }

	    [Scenario]
	    public async Task Create_new_project_with_default_name()
	    {
	        await Runner.AddSteps(
	            _ => Given_is_an_empty_workspace_with_name("Root"),
	            _ => When_I_create_a_new_project(),
	            _ => Then_the_project_NAME_was_created_inside_root_folder("Root")).RunAsyncWithTimeout();
	    }

        [Scenario]
	    public async Task Create_new_project_with_specific_name_in_specific_folder()
	    {
	        await Runner.AddSteps(
	            _ => Given_is_an_empty_workspace(),
	            _ => When_I_create_a_new_project_with_name_in_folder("NewProject", "Fooba"),
	            _ => Then_the_project_NAME_was_created_in_folder("NewProject", "Fooba")).RunAsyncWithTimeout();
	    }

	    [Scenario]
	    public async Task Create_new_project_in_existing_folder()
	    {
	        await Runner.AddSteps(
	            _ => Given_is_a_new_project_NAME("NewProject"),
	            _ => When_I_create_a_new_project_with_name("NewProject"),
                _ => Then_the_user_was_informed_that_the_artifact_exists_already()).RunAsyncWithTimeout();
	    }

	    [Scenario]
	    public async Task Create_new_project_in_existing_folder_forced()
	    {
	        await Runner.AddSteps(
	            _ => Given_is_a_new_project_NAME("NewProject"),
	            _ => When_I_create_a_new_project_with_name_forced("NewProject"),
	            _ => Then_the_project_NAME_was_again_created("NewProject")).RunAsyncWithTimeout();
	    }

        [Scenario]
        public async Task Create_new_project_creates_library_component_and_program()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project(),
                _ => Then_the_project_contains_component_and_program_cpp_and_hpp_files()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_project_creates_component_with_specific_name()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_componentname("NewComponent"),
                _ => Then_the_project_contains_a_component_with_name("NewComponent")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_project_creates_program_with_specific_name()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_programname("NewProgram"),
                _ => Then_the_project_contains_a_program_with_name("NewProgram")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_project_creates_component_and_program_with_correct_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => Then_the_components_namespace_starts_with_namespace("DComponent", "A.B.C.D"),
                _ => Then_the_programs_namespace_starts_with_namespace("DProgram", "A.B.C.D")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_acf_component_creates_component()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_acfproject_with_componentname("MyProjectComponent"),
                _ => Then_the_project_contains_an_acfcomponent_with_name("MyProjectComponent")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_acfconfig_on_new_acfproject()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_acfproject_with_componentname("MyAcfComponent"),
                _ => Then_the_acfconfig_file_is_created_with_the_component("Project", "MyAcfComponent")
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_help_files_for_consumable_libraries()
        {
	        await Runner.AddSteps(
		        _ => Given_is_an_empty_workspace(),
		        _ => When_I_create_a_new_consumable_library_with_the_name("FancyLibrary"),
		        _ => Then_the_files_exist_in_location("FancyLibrary/How_to_use.txt","FancyLibrary/include/ADD_PUBLIC_HEADERS_HERE.txt","FancyLibrary/external/ADD_DEPENDENT_LIBRARIES_HERE.txt")
	        ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_shared_native_project()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_shared_native_project_with_name("SharedNativeTest"),
                _ => Then_the_files_exist_in_location("SharedNativeTest/SharedNativeTestCpp/CMakeLists.txt", "SharedNativeTest/SharedNativeTestCpp/SharedNativeTestCpp.vcxproj",
                                                      "SharedNativeTest/SharedNativeTestCpp/plcnext.proj", "SharedNativeTest/SharedNativeTestCSharp/SharedNativeTestCSharp.csproj")
                ).RunAsyncWithTimeout();
        }

        public Project_Creation_Feature(ITestOutputHelper helper) : base(helper)
        {
        }
	}
}