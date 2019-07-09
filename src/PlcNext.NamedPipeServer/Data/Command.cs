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
using Newtonsoft.Json.Linq;

namespace PlcNext.NamedPipeServer.Data
{
    internal class Command
    {
        public Command(string rawCommand)
        {
            RawCommand = rawCommand;
        }

        private Command(Command other, bool result, bool? canceled = null)
        {
            RawCommand = other.RawCommand;
            ParsedCommand = other.ParsedCommand;
            Result = result;
            IsCanceled = canceled ?? other.IsCanceled;
            DetailedResult = other.DetailedResult;
        }

        private Command(Command other, string parsedCommand)
        {
            RawCommand = other.RawCommand;
            ParsedCommand = parsedCommand;
            Result = other.Result;
            IsCanceled = other.IsCanceled;
            DetailedResult = other.DetailedResult;
        }

        private Command(Command other, JObject detailedResult)
        {
            RawCommand = other.RawCommand;
            ParsedCommand = other.ParsedCommand;
            Result = other.Result;
            IsCanceled = other.IsCanceled;
            DetailedResult = detailedResult;
        }

        public string RawCommand { get; }

        public string ParsedCommand { get; }

        public bool Result { get; }

        public bool IsCanceled { get; }
        
        public JObject DetailedResult { get; }

        public Command WithParsedCommand(string parsedCommand)
        {
            return new Command(this, parsedCommand);
        }

        public Command WithResult(bool result)
        {
            return new Command(this, result);
        }

        public Command WithDetailedResult(JObject detailedResult)
        {
            return new Command(this, detailedResult);
        }

        public Command AsCanceled()
        {
            return new Command(this, false, true);
        }

        public override string ToString()
        {
            return $"{nameof(RawCommand)}: {RawCommand}, {nameof(ParsedCommand)}: {ParsedCommand}, {nameof(Result)}: {Result}, {nameof(IsCanceled)}: {IsCanceled}";
        }
    }
}
