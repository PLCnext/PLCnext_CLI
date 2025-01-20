#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.Collections.Generic;

namespace Test.PlcNext.Tools
{
    public class CompmetaData
    {
        public CompmetaData(string componentName, string[] path, IEnumerable<Portmeta> portmetas, IEnumerable<string> programs)
        {
            ComponentName = componentName;
            Portmetas = portmetas;
            Path = path;
            Programs = programs;
        }

        public string ComponentName { get; }
        public string[] Path { get; }
        public IEnumerable<Portmeta> Portmetas { get; }
        public IEnumerable<string> Programs { get; }
    }
}
