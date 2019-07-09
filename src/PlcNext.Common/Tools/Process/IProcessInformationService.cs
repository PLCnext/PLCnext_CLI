#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace PlcNext.Common.Tools.Process
{
    public interface IProcessInformationService
    {
        IEnumerable<int> GetOtherInstancesProcessIds();
    }
}