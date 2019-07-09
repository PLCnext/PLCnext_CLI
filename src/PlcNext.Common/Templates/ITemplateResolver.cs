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
using System.Threading.Tasks;
using PlcNext.Common.DataModel;

namespace PlcNext.Common.Templates
{
    public interface ITemplateResolver
    {
        string Resolve(string stringToResolve, IEntityBase dataSource);
        Task<string> ResolveAsync(string stringToResolve, IEntityBase dataSource);
    }
}
