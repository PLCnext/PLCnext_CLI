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

namespace PlcNext.NamedPipeServer.Communication
{
    public interface IInstanceMessageSender
    {
        void SendSettingsUpdated(Action messageCompletedAction = null);
        void SendProjectSettingsUpdate(string path, Action messageCompletedAction = null);
        void SendSdksUpdated(Action messageCompletedAction = null);
    }
}