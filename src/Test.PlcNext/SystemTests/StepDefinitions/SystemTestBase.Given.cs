#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Xunit;
#pragma warning disable 4014

namespace Test.PlcNext.SystemTests.StepDefinitions
{
	public abstract partial class SystemTestBase
	{
	    protected void Given_is_an_empty_workspace()
	    {
            //nothing to do
	    }

	    protected void Given_are_the_standard_settings()
	    {
	        //nothing to do - standard settings automatically loaded
	    }

        protected void Given_is_the_project(string project)
	    {
	        ScenarioContext.LoadProject(project);
        }

        protected void Given_is_the_project_with_the_additional_library_in_the_directory(string project, string directory, string library)
        {
            ScenarioContext.LoadProject(project);
            ScenarioContext.LoadLibrary(library, directory);
        }


        protected void Given_is_that_the_SDK_is_missing()
	    {
	        ScenarioContext.RemoveSdk();
	    }

	    protected void Given_is_that_the_public_key_for_the_cli_repository_is_missing()
	    {
	        ScenarioContext.RemoveApplicationFile("public_cli_repository_key.xml");
	    }

        protected CompositeStep Given_is_a_new_project_NAME(string name)
	    {
	        return CompositeStep.DefineNew()
	                            .AddSteps(_ => When_I_create_a_new_project_with_name(name),
	                                      _ => Then_the_project_NAME_was_created(name))
	                            .Build();
	    }
        
	    protected void Given_is_an_empty_workspace_with_name(string name)
	    {
            //ScenarioContext.SetWorkspaceName(name);
	    }

	    protected void Given_is_the_working_directory_PATH(string path)
	    {
	        ScenarioContext.SetCurrentDirectory(path);
        }

	    protected CompositeStep Given_are_settings_with_the_modification_KEY_VALUE(string key, string value)
	    {
	        return CompositeStep.DefineNew()
	                            .AddSteps(_ => Given_are_the_standard_settings(),
	                                      _ => When_I_change_the_setting_to_the_value(key, value))
	                            .Build();
        }

	    protected CompositeStep Given_are_settings_with_the_cleared_setting_KEY(string key)
	    {
	        return CompositeStep.DefineNew()
	                            .AddSteps(_ => Given_are_the_standard_settings(),
	                                      _ => When_I_clear_the_setting(key))
	                            .Build();
        }

	    protected void Given_is_that_the_update_repository_file_is_unreachable()
	    {
	        ScenarioContext.RemoveFromWebServer("cli/repository.xml");
        }

	    protected void Given_is_that_the_update_repository_file_certificate_is_unreachable()
	    {
	        ScenarioContext.RemoveFromWebServer("cli/repository.xml.cer");
        }

        protected void Given_is_a_manipulated_repository_file()
	    {
	        ScenarioContext.ChangeFileOnWebServer("Test.PlcNext.Deployment.ManipulatedRepository.xml", "cli/repository.xml");
        }

	    protected void Given_is_a_repository_file_with_only_older_versions()
	    {
	        ScenarioContext.ChangeFileOnWebServer("Test.PlcNext.Deployment.OnlyOlderRepository.xml", "cli/repository.xml");
	        ScenarioContext.ChangeFileOnWebServer("Test.PlcNext.Deployment.OnlyOlderRepository.xml.cer", "cli/repository.xml.cer");
        }

	    protected CompositeStep Given_is_a_different_library_builder_location()
	    {
	        string newLocation = ScenarioContext.ChangeLibraryBuilderLocation();
	        return CompositeStep.DefineNew()
	                            .AddSteps(_ => Given_are_the_standard_settings(),
	                                      _ => When_I_change_the_setting_to_the_value("LibraryBuilderLocation", newLocation))
	                            .Build();
        }

	    protected void Given_is_that_the_library_builder_is_missing()
	    {
	        ScenarioContext.RemoveLibraryBuilder();
	    }

        protected void Given_is_that_the_cmake_build_system_exists_for_targets(params string[] targets)
        {
            ScenarioContext.CreateCMakeBuildSystem(targets);
        }

        protected void Given_is_that_the_directory_exists(string relativeDirectoryPath)
        {
            ScenarioContext.CreateDirectory(relativeDirectoryPath);
        }

        protected void Given_is_that_the_file_exists(string relativeFilePath)
        {
            ScenarioContext.CreateFile(relativeFilePath);
        }

        protected void Given_cmake_returns_a_code_model_with_the_following_libraries(string projectName, params string[] libraries)
        {
            ScenarioContext.SetCodeModel(projectName, libraries);
        }

        protected void Given_cmake_returns_a_code_model_with_the_following_include_paths(string projectName, params string[] includePaths)
        {
            ScenarioContext.SetCodeModel(projectName, includePaths:includePaths);
        }
    }
}