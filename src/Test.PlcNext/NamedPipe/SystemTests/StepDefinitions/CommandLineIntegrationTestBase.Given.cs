#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class CommandLineIntegrationTestBase
    {
        protected void Given_is_the_project_with_set_default_path(string project)
        {
            ScenarioContext.CommandLineContext.LoadProject(project);
            ScenarioContext.CommandLineContext.SetCurrentDirectory(project);
        }
        
        protected void Given_is_the_project(string project)
        {
            ScenarioContext.CommandLineContext.LoadProject(project);
        }
    }
}