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

#pragma warning disable 4014

namespace Test.PlcNext.NamedPipe.SystemTests.StepDefinitions
{
    public abstract partial class SystemTestBase
    {
        protected CompositeStep Given_is_a_connected_client()
        {
            return CompositeStep.DefineNew()
                                .AddSteps(_ => Given_is_a_started_server(),
                                          _ => When_I_connect_with_a_valid_handshake(),
                                          _ => Then_the_the_client_received_a_successful_handshake_reply())
                                .Build();
        }

        protected CompositeStep Given_is_a_connected_client_without_handshake()
        {
            return CompositeStep.DefineNew()
                                .AddSteps(_ => Given_is_a_started_server(),
                                          _ => When_I_connect_without_a_handshake())
                                .Build();
        }

        protected CompositeStep Given_is_a_connected_client_with_wrong_handshake()
        {
            return CompositeStep.DefineNew()
                                .AddSteps(_ => Given_is_a_started_server(),
                                          _ => When_I_connect_with_a_wrong_handshake())
                                .Build();
        }

        protected async Task Given_is_a_started_server()
        {
            await ScenarioContext.StartServer(false);
        }

        protected async Task Given_is_a_started_server_with_heartbeat_enabled()
        {
            await ScenarioContext.StartServer(true);
        }

        protected CompositeStep Given_is_a_started_server_with_heartbeat_enabled_and_valid_handshake()
        {
            return CompositeStep.DefineNew()
                                .AddSteps(_ => Given_is_a_started_server_with_heartbeat_enabled(),
                                          _ => When_I_connect_with_a_valid_handshake(),
                                          _ => Then_the_the_client_received_a_successful_handshake_reply())
                                .Build();
        }
    }
}
