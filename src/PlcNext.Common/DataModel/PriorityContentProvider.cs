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
using PlcNext.Common.Tools.Priority;

namespace PlcNext.Common.DataModel
{
    public abstract class PriorityContentProvider : IEntityContentProvider, IPrioritySubject
    {
        public abstract bool CanResolve(Entity owner, string key, bool fallback = false);
        public abstract Entity Resolve(Entity owner, string key, bool fallback = false);

        public virtual SubjectIdentifier SubjectIdentifier => ThisSubjectIdentifier.From(this);
        public virtual SubjectIdentifier HigherPrioritySubject { get; } = SubjectIdentifier.None;
        public virtual SubjectIdentifier LowerPrioritySubject { get; } = SubjectIdentifier.None;
    }
}
