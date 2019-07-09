#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;

namespace PlcNext.Common.Tools.Events
{
    public class Change
    {
        private readonly Action invertAction;
        private readonly string description;

        public Change(Action invertAction, string description = "")
        {
            this.invertAction = invertAction;
            this.description = description;
        }

        public void Invert()
        {
            invertAction();
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;
            return $"Change: {description}";
        }
    }
}
