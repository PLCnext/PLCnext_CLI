#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlcNext.Common.Tools.FileSystem;
using PlcNext.Common.Tools.Process;

namespace PlcNext.Common.Tools.SDK;

internal class CMakeConversation : ICMakeConversation
{
    const string CmakeApiSubDirectory = "\\.cmake\\api\\v1\\";
    const string CmakeApiQuerySubDirectory = CmakeApiSubDirectory + "query\\";
    const string CmakeApiReplySubDirectory = CmakeApiSubDirectory + "reply\\";
    const string CmakeFileApiCodemodelQuery = "codemodel-v2";
    const string CmakeFileApiCacheQuery =  "cache-v2";
    
    private readonly ExecutionContext executionContext;
    private readonly IProcessManager processManager;
    private readonly IBinariesLocator binariesLocator;

    public CMakeConversation(ExecutionContext executionContext, IProcessManager processManager,
        IBinariesLocator binariesLocator)
    {
        this.executionContext = executionContext;
        this.processManager = processManager;
        this.binariesLocator = binariesLocator;
    }
    
    public async Task<JObject> GetCodeModel(string projectName,
        VirtualDirectory binaryDirectory)
    {
        string cmakeFileApiReplyDirectory = await CreateCmakeFileApiQuery(binaryDirectory, CmakeFileApiCodemodelQuery).ConfigureAwait(true);

        await RunCmake(binaryDirectory).ConfigureAwait(true);

        string targetFileName = Directory.GetFiles(cmakeFileApiReplyDirectory,
                $"target-{projectName}-*", SearchOption.TopDirectoryOnly).FirstOrDefault();

        return await DeserializeResult(targetFileName).ConfigureAwait(true);
    }

    public async Task<JObject> GetCache(VirtualDirectory binaryDirectory)
    {
        string cmakeFileApiReplyDirectory = await CreateCmakeFileApiQuery(binaryDirectory, CmakeFileApiCacheQuery).ConfigureAwait(true);

        await RunCmake(binaryDirectory).ConfigureAwait(true);
        
        string targetFileName = Directory.GetFiles(cmakeFileApiReplyDirectory,
                $"cache-v2-*", SearchOption.TopDirectoryOnly).FirstOrDefault();
            
        return await DeserializeResult(targetFileName).ConfigureAwait(true);
    }
    
    static async Task<string> CreateCmakeFileApiQuery(VirtualDirectory binaryDirectory, string cmakeFileApiQuery)
    {
        string cmakeFileApiQueryDirectory = binaryDirectory.FullName + CmakeApiQuerySubDirectory;
        string cmakeFileApiReplyDirectory = binaryDirectory.FullName + CmakeApiReplySubDirectory;
        if (Directory.Exists(cmakeFileApiQueryDirectory))
        {
            Directory.Delete(cmakeFileApiQueryDirectory, recursive: true);
        }
        if (Directory.Exists(cmakeFileApiReplyDirectory))
        {
            Directory.Delete(cmakeFileApiReplyDirectory, recursive: true);
        }
        Directory.CreateDirectory(cmakeFileApiQueryDirectory);
        await using (File.Create(cmakeFileApiQueryDirectory + cmakeFileApiQuery))
        {
        }
        return cmakeFileApiReplyDirectory;
    }
    
    async Task RunCmake(VirtualDirectory binaryDirectory)
    {
        await processManager.StartProcess(binariesLocator.GetExecutableCommand("cmake"), binaryDirectory.FullName,
            executionContext).WaitForExitAsync().ConfigureAwait(true);
    }
    
    private static async Task<JObject> DeserializeResult(string targetFileName)
    {
        if (targetFileName == null) return new JObject();

        FileStream fileStream = new FileStream(targetFileName, FileMode.Open);
        using StreamReader reader = new StreamReader(fileStream);
        using JsonReader jsonReader = new JsonTextReader(reader);
        return await JObject.LoadAsync(jsonReader).ConfigureAwait(true);
    }
}