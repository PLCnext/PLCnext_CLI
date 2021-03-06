﻿#region Copyright
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

namespace PlcNext.Common.Commands
{
    public class UpdateTargetsCommandArgs : CommandArgs
    {
        public UpdateTargetsCommandArgs(string path, bool downgrade)
        {
            Path = path;
            Downgrade = downgrade;
        }

        public string Path { get; }

        public bool Downgrade { get; }
    }
}
