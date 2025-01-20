#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Test.PlcNext.SystemTests.StepDefinitions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"User creates new program via CLI with different parameters.")]
    public class Program_Creation_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Create_new_program_with_specific_name()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_program_with_name_for_component("NewProgram", "MyComponent", false),
                _ => Then_the_project_contains_a_program_with_name("NewProgram"),
                _ => Then_the_user_was_informed_that_the_program_was_created_successfully(),
                _ => Then_the_programs_namespace_starts_with_namespace("NewProgram", "Standard")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_with_default_name()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardNew"),
                _ => Given_is_the_working_directory_PATH("StandardNew"),
                _ => When_I_create_a_new_program_for_component("StandardComponent", false, false),
                _ => Then_the_user_was_informed_that_the_artifact_exists_already()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_for_project_in_specific_folder()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_create_a_new_program_with_name_for_component_for_project_in_specific_folder("NewProgram", "MyComponent", false),
                _ => Then_the_project_contains_a_program_with_name("NewProgram"),
                _ => Then_the_user_was_informed_that_the_program_was_created_successfully()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_with_existing_name_forced()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_program_for_component("MyComponent", true, false),
                _ => Then_the_project_contains_a_program_with_name("MyProgram"),
                _ => Then_the_code_entity_was_created(),
                _ => Then_the_user_was_informed_that_the_program_was_created_successfully()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_without_specifiying_component()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_I_create_a_new_program(false),
                _ => Then_the_user_was_informed_that_the_component_option_is_missing()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_creates_program_with_correct_namespaces()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_program_for_component("DComponent", true, true),
                _ => Then_the_user_was_informed_that_the_program_was_created_successfully(),
                _ => Then_the_programs_namespace_starts_with_namespace("DProgram", "A.B.C.D")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_with_custom_relative_path()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_program_with_path_for_component("customfolder", true, false, "DComponent"),
                _ => Then_the_program_exists_in_path("DProgram", "A.B.C.D/src/customfolder")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_with_custom_absolute_path()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_project_with_name("A.B.C.D"),
                _ => When_I_create_a_new_program_with_path_for_component(Path.GetFullPath("A.B.C.D"), true, false, "MyComponent"),
                _ => Then_the_program_exists_in_path("DProgram", "A.B.C.D")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_without_proj()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => Given_is_the_working_directory_PATH("Standard"),
                _ => When_I_create_a_new_program_with_name_for_component("NewProgram", "MyComponent", false),
                _ => Then_the_user_was_informed_that_the_program_was_created_successfully(),
                _ => Then_the_program_exists_in_path("NewProgram", "src")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_without_proj_and_without_codefiles()
        {
            await Runner.AddSteps(
                _ => Given_is_an_empty_workspace(),
                _ => When_I_create_a_new_program_for_component("MyComponent", false, false),
                _ => Then_the_user_was_informed_that_the_artifact_was_not_found()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_in_project_with_duplicate_component_name()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentsWithSameName"),
                _ => When_I_create_a_new_program_for_component("MyComponent", false, false),
                _ => Then_the_user_was_informed_that_the_component_name_is_ambiguous()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_for_pseudo_ambiguous_component_with_full_name()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("PseudoAmbiguousComponents"),
                _ => Given_is_the_working_directory_PATH("PseudoAmbiguousComponents"),
                _ => When_I_create_a_new_program_with_name_for_component("MyProgram", "PseudoAmbiguousComponents::MyComp", false),
                _ => Then_the_project_contains_a_program_with_name("MyProgram")).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_in_project_with_component_name_as_prefix_of_existing_component_names()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("TwoComponentsAndPrograms"),
                _ => When_I_create_a_new_program_for_component("Component", false, true),
                _ => Then_the_user_was_informed_that_the_component_name_is_ambiguous()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Create_new_program_for_component_in_different_source_folder()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentsInMultipleSources"),
                _ => Given_is_the_working_directory_PATH("ComponentsInMultipleSources"),
                _ => When_I_create_a_new_program_with_name_for_component_with_source_folders("NewProgram", "ComponentsInMultipleNamespaces::MultipleNamespacesComponent", "src", "extern"),
                _ => Then_the_project_contains_a_program_with_name("NewProgram"),
                _ => Then_the_programs_namespace_starts_with_namespace("NewProgram", "ComponentsInMultipleNamespaces")).RunAsyncWithTimeout();
        }
    }
}
