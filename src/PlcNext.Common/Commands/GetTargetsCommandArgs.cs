#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright PHOENIX CONTACT Software GmbH
//
///////////////////////////////////////////////////////////////////////////////
#endregion


namespace PlcNext.Common.Commands
{
    public class GetTargetsCommandArgs : CommandArgs
    {
        public GetTargetsCommandArgs(string path, bool shortVersion)
        {
            Path = path;
            ShortVersion = shortVersion;
        }

        public string Path { get; }
        public bool ShortVersion { get; }
    }
}
