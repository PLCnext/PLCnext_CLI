#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.Events;
using PlcNext.Common.Tools.Process;
using PlcNext.Common.Tools.SDK;
using PlcNext.Common.Tools.Settings;
using PlcNext.NamedPipeServer.Communication;
using ExecutionContext = PlcNext.Common.Tools.ExecutionContext;

#pragma warning disable 4014

namespace PlcNext.CliNamedPipeMediator
{
    public class UpdateMessagesMediator : IDisposable
    {
        private readonly ISettingsObserver settingsObserver;
        private readonly IProcessInformationService processInformationService;
        private readonly IInstanceCommunicationService instanceCommunicationService;
        private readonly ExecutionContext executionContext;
        private readonly ISdkRepository sdkRepository;
        private readonly CancellationToken cancellationToken;
        private UpdateChangeObserver currentObserver;

        public UpdateMessagesMediator(ISettingsObserver settingsObserver, IProcessInformationService processInformationService, IInstanceCommunicationService instanceCommunicationService, ExecutionContext executionContext, ISdkRepository sdkRepository, CancellationToken cancellationToken)
        {
            this.settingsObserver = settingsObserver;
            this.processInformationService = processInformationService;
            this.instanceCommunicationService = instanceCommunicationService;
            this.executionContext = executionContext;
            this.sdkRepository = sdkRepository;
            this.cancellationToken = cancellationToken;
            settingsObserver.SettingAdded += SettingsObserverOnSettingChanged;
            settingsObserver.SettingCleared += SettingsObserverOnSettingChanged;
            settingsObserver.SettingRemoved += SettingsObserverOnSettingChanged;
            settingsObserver.SettingSet += SettingsObserverOnSettingChanged;
            executionContext.ObservableRegistered += ExecutionContextOnObservableRegistered;
            executionContext.ObservableUnregistered += ExecutionContextOnObservableUnregistered;
            sdkRepository.Updated += SdkRepositoryOnUpdated;
        }

        private void SdkRepositoryOnUpdated(object sender, EventArgs e)
        {
            SendUpdate((messageSender, wait) => messageSender.SendSdksUpdated(() => wait.Set()));
        }

        private void ExecutionContextOnObservableUnregistered(object sender, EventArgs e)
        {
            currentObserver?.Dispose();
        }

        private void ExecutionContextOnObservableRegistered(object sender, EventArgs e)
        {
            currentObserver = new UpdateChangeObserver(this);
        }

        private void SettingsObserverOnSettingChanged(object sender, SettingsObserverEventArgs e)
        {
            SendUpdate((messageSender, wait) => messageSender.SendSettingsUpdated(() => wait.Set()));
        }

        private async Task SendUpdate(Action<IInstanceMessageSender, AutoResetEventAsync> sendAction)
        {
            foreach (int otherInstancesProcessId in processInformationService.GetOtherInstancesProcessIds())
            {
                executionContext.WriteVerbose($"Sending update message to instance with process id {otherInstancesProcessId}.");
                try
                {
                    using (ITemporaryCommunicationChannel communicationChannel =
                        await instanceCommunicationService.OpenCommunicationChannel(otherInstancesProcessId))
                    {
                        AutoResetEventAsync waitEvent = new AutoResetEventAsync(false);
                        sendAction(communicationChannel.MessageSender,waitEvent);
                        await waitEvent.WaitAsync(cancellationToken);
                        executionContext.WriteVerbose($"Update message send.");
                    }
                }
                catch (Exception exception)
                {
                    executionContext.WriteWarning(
                        $"Could not send update to other instance with process id {otherInstancesProcessId}.{Environment.NewLine}" +
                        $"{exception}", false);
                }
            }
        }

        public void Dispose()
        {
            settingsObserver.SettingAdded -= SettingsObserverOnSettingChanged;
            settingsObserver.SettingCleared -= SettingsObserverOnSettingChanged;
            settingsObserver.SettingRemoved -= SettingsObserverOnSettingChanged;
            settingsObserver.SettingSet -= SettingsObserverOnSettingChanged;
            executionContext.ObservableRegistered -= ExecutionContextOnObservableRegistered;
            executionContext.ObservableUnregistered -= ExecutionContextOnObservableUnregistered;
            sdkRepository.Updated -= SdkRepositoryOnUpdated;
            currentObserver?.Dispose();
        }

        private class UpdateChangeObserver : IObserver<Change>, IDisposable
        {
            private readonly UpdateMessagesMediator mediator;
            private readonly IDisposable subscriptionToken;

            public UpdateChangeObserver(UpdateMessagesMediator mediator)
            {
                this.mediator = mediator;
                subscriptionToken = mediator.executionContext.Observable.Subscribe(this);
            }


            public void OnCompleted()
            {
                //Do nothing
            }

            public void OnError(Exception error)
            {
                //Do nothing
            }

            public void OnNext(Change value)
            {
                if (value is ProjectSettingChange projectSettingChange)
                {
                    mediator.SendUpdate((messageSender, wait) => messageSender.SendProjectSettingsUpdate(
                                            projectSettingChange.Path,
                                            () => wait.Set()));
                }
            }

            public void Dispose()
            {
                subscriptionToken?.Dispose();
            }
        }
    }
}