#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Threading.Tasks;
using Test.PlcNext.SystemTests.Tools;

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class CommandLineIntegrationTestBase
    {
        protected async Task When_I_change_the_setting_to_the_value(string setting, string value)
        {
            await ScenarioContext.CommandLineContext.ChangeSetting(setting, value, SystemTestContext.SettingChange.Change);
        }

        protected async Task When_I_add_the_target(string target, string version)
        {
            await ScenarioContext.CommandLineContext.ChangeTarget(target, version, SystemTestContext.TargetChange.Add);
        }

        protected async Task When_I_explore_all_sdks()
        {
            await ScenarioContext.CommandLineContext.ExploreSdks();
        }
    }
}