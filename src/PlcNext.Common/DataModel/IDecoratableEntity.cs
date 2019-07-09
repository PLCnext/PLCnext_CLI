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

namespace PlcNext.Common.DataModel
{
    public interface IDecoratableEntity
    {
        IDecoratableEntity DecoratedEntity { get; }
        IDecoratableEntity DecoratedByEntity { get; set; }
    }
}
