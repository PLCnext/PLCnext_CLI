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

namespace PlcNext.Common.Tools.Priority
{
    public class ThisSubjectIdentifier : SubjectIdentifier
    {
        private ThisSubjectIdentifier(Type identifier) : base(identifier.Name)
        {
        }

        public static ThisSubjectIdentifier From(IPrioritySubject subject) => new ThisSubjectIdentifier(subject?.GetType());
    }
}
