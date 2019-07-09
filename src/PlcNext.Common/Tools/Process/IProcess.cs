#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Threading.Tasks;

namespace PlcNext.Common.Tools.Process
{
    internal interface IProcess : IDisposable
    {
        Task WaitForExitAsync();
        void WaitForExit();
        int ExitCode { get; }
    }
}
