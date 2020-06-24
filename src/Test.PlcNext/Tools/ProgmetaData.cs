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

namespace Test.PlcNext.Tools
{
    public class ProgmetaData
    {
        public ProgmetaData(string programName, string[] path, IEnumerable<Portmeta> portmetas)
        {
            ProgramName = programName;
            Portmetas = portmetas;
            Path = path;
        }

        public string ProgramName { get; }
        public string[] Path { get; }
        public IEnumerable<Portmeta> Portmetas { get; }
    }

    public class Portmeta
    {
        public Portmeta(string name, string type, string attributes)
        {
            Name = name;
            Type = type;
            Attributes = attributes;
        }

        public Portmeta(string name, string type, string attributes, string multiplicity) : this(name, type, attributes)
        {
            Multiplicity = multiplicity;
            MultiplicityUsed = true;
        }
        public string Name { get; }
        public string Type { get; }
        public string Attributes { get; }

        public string Multiplicity { get; }

        public bool MultiplicityUsed { get; }
    }
}
