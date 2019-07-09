#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using PlcNext.Common.Tools.Events;

namespace PlcNext.Common.Tools
{
    public interface ITransactionFactory
    {
        ITransaction StartTransaction(out ChangeObservable observable);
    }

    public interface ITransaction : IDisposable
    {
        void OnCompleted();
    }
}
