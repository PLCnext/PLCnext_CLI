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
using PlcNext.Common.Tools;
using Test.PlcNext.SystemTests.StepDefinitions;

#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.Features
{
    [FeatureDescription(@"Transactions revert everything back.")]
    public class Transaction_Feature : MockedSystemTestBase
    {
        [Scenario]
        public async Task Rollback_all_created_objects_after_creation()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentInSourceFolder"),
                _ => When_I_create_a_new_project_with_name_and_componentname("ComponentInSourceFolder", "Component"),
                _ => Then_no_files_were_changed_or_added_to_the_workspace()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Rollback_does_not_delete_initial_files()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("ComponentInSourceFolder"),
                _ => When_I_create_a_new_project_with_name_and_componentname("ComponentInSourceFolder", "Component"),
                _ => Then_existing_files_were_not_deleted()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Rollback_all_created_objects_after_generation()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_the_file_system_throws_an_error_on_accessing_the_file(
                    Path.Combine("Standard", Constants.IntermediateFolderName,
                                 Constants.MetadataFolderName, "Standard.typemeta")),
                _ => When_I_generate_all_metafiles_without_auto_detection(),
                _ => Then_no_files_were_changed_or_added_to_the_workspace()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Rollback_restores_metadata_folder()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("EmptyWithLibmeta"),
                _ => When_the_file_system_throws_an_error_on_accessing_the_file(
                    Path.Combine("EmptyWithLibmeta", Constants.IntermediateFolderName, 
                                 Constants.MetadataFolderName, "EmptyWithLibmeta.typemeta")),
                _ => When_I_generate_all_metafiles(),
                _ => Then_existing_files_were_not_deleted()).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Rollback_restores_build_folder()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("StandardWithBuildArtifacts"),
                _ => When_the_processManager_starts_a_process_that_throws_an_error(),
                _ => When_I_build_the_project(),
                _ => Then_existing_files_were_not_deleted(),
                _ => Then_only_the_cmake_file_was_added_to_the_workspace()
                ).RunAsyncWithTimeout();
        }

        [Scenario]
        public async Task Rollback_all_created_objects_after_build()
        {
            await Runner.AddSteps(
                _ => Given_is_the_project("Standard"),
                _ => When_the_processManager_starts_a_process_that_throws_an_error(),
                _ => When_I_build_the_project(),
                _ => Then_only_the_cmake_file_was_added_to_the_workspace()
                ).RunAsyncWithTimeout();
        }
    }
}
