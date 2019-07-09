#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.Project;
using PlcNext.Common.Tools;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Build
{
    internal class ExternalLibrariesParser
    {
        private static readonly Regex TargetVersionLocationParser = new Regex(@"^(?<name>[^,]+),(?<version>[^,]+),(?<libraries>.+)$", RegexOptions.Compiled);
        private static readonly Regex LibrariesDecoder = new Regex("(?<element>\"[^\"]+\"|[^,]+)", RegexOptions.Compiled);

        public static Dictionary<Target, IEnumerable<VirtualFile>> ParseExternalLibraries(IEnumerable<string> rawExternalLibraries, ITargetParser targetParser, 
                                                                                            IFileSystem fileSystem, IEnumerable<Target> targetBase)
        {
            Dictionary<Target, IEnumerable<VirtualFile>> externalLibs = new Dictionary<Target, IEnumerable<VirtualFile>>();
            foreach (string externalLibrariesItem in rawExternalLibraries)
            {
                ParseExternalLibrary(externalLibrariesItem);
            }
            return externalLibs;

            void ParseExternalLibrary(string externalLibrariesItem)
            {
                List<string> elements = new List<string>();

                Match match = LibrariesDecoder.Match(externalLibrariesItem);
                while (match.Success)
                {
                    elements.Add(match.Groups["element"].Value);

                    match = match.NextMatch();
                }
                if (elements.Count == 0)
                {
                    throw new FormattableException("Could not parse external libraries input." +
                                                   "Expected pattern is [<target>,[<version>,]]\"Path/To/ExternalLib.so\"");
                }

                if (elements.Count == 1)
                {
                    //no targetspecific external libraries allowed in this case
                    if(rawExternalLibraries.Count() > 1)
                    {
                        throw new WrongCombinedExternalLibrariesException();
                    }

                    addElementToLibs(elements[0]);

                    return;
                }

                //check if first element is file
                if (fileSystem.FileExists(elements[0]))
                {
                    //all elements are files
                    //no targetspecific external libraries allowed in this case
                    if (rawExternalLibraries.Count() > 1)
                    {
                        throw new WrongCombinedExternalLibrariesException();
                    }

                    foreach (string element in elements)
                    {
                        addElementToLibs(element);
                    }
                    return;
                }

                //check if second element is file
                if (fileSystem.FileExists(elements[1]))
                {
                    //second element is file,
                    //try to parse first element as target
                    try
                    {
                        Target target = targetParser.ParseTarget(elements[0], null, targetBase);
                        elements.RemoveAt(0);

                        List<VirtualFile> libs = new List<VirtualFile>();
                        foreach (string element in elements)
                        {
                            if (!fileSystem.FileExists(element))
                            {
                                throw new LibraryNotFoundException(element);
                            }
                            libs.Add(fileSystem.GetFile(element));
                        }
                        if (externalLibs.ContainsKey(target))
                        {
                            throw new FormattableException($"For the target {target.GetLongFullName()} external libraries are specified twice.");
                        }
                        externalLibs.Add(target, libs);
                        return;
                    }
                    catch (TargetNameNotFoundException e)
                    {
                        throw new MalformedExternalLibrariesOptionException(elements[0], "target", e.Message);
                    }
                }

                //assume that second element is version

                try
                {
                    Target target = targetParser.ParseTarget(elements[0], elements[1], targetBase);
                    elements.RemoveAt(1);
                    elements.RemoveAt(0);

                    List<VirtualFile> libs = new List<VirtualFile>();
                    foreach (string element in elements)
                    {
                        if (!fileSystem.FileExists(element))
                        {
                            throw new LibraryNotFoundException(element);
                        }
                        libs.Add(fileSystem.GetFile(element));
                    }
                    if (externalLibs.ContainsKey(target))
                    {
                        throw new FormattableException($"For the target {target.GetLongFullName()} external libraries are specified twice.");
                    }
                    externalLibs.Add(target, libs);
                    return;
                }
                catch (TargetNameNotFoundException e)
                {
                    throw new MalformedExternalLibrariesOptionException(elements[0], "target", e.Message);
                }
                catch (TargetVersionNotFoundException e)
                {
                    throw new MalformedExternalLibrariesOptionException(elements[1],
                                                                        $"version for target{elements[0]}", e.Message);
                }


                void addElementToLibs(string element)
                {
                    if (!fileSystem.FileExists(element))
                    {
                        throw new LibraryNotFoundException(element);
                    }
                    foreach (Target t in targetBase)
                    {
                        List<VirtualFile> libse = new List<VirtualFile>();
                        libse.Add(fileSystem.GetFile(element));
                        if (externalLibs.ContainsKey(t))
                        {
                            throw new FormattableException($"For the target {t.GetLongFullName()} external libraries are specified twice.");
                        }
                        externalLibs.Add(t, libse);
                    }
                }
            }

        }
    }
}
