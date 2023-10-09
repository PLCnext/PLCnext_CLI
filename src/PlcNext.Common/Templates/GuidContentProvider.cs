#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using PlcNext.Common.CodeModel;
using PlcNext.Common.DataModel;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PlcNext.Common.Templates
{
    internal class GuidContentProvider : PriorityContentProvider
    {

        private readonly Regex guidRegex = new Regex(@"guid(?<identifier>\[.+\])?", RegexOptions.Compiled);

        public override bool CanResolve(Entity owner, string key, bool fallback = false)
        {
            return guidRegex.IsMatch(key);
        }

        public override Entity Resolve(Entity owner, string key, bool fallback = false)
        {
            Match match = guidRegex.Match(key);
            if (match.Success)
                return owner.Create(key, GenerateGuid(match.Groups["identifier"].Value, owner));
            throw new ContentProviderException(key, owner);
        }

        private static string GenerateGuid(string value, Entity owner)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Guid.NewGuid().ToString();
            }

            value = value.Trim('[', ']', ' ');
            CodeEntity codeEntity = CodeEntity.Decorate(owner.Root);
            string projectName = $"{codeEntity.Namespace}.{codeEntity.Name}";
            byte[] bytes = Encoding.ASCII.GetBytes(value + projectName);
            byte[] hash = ((HashAlgorithm) CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);
            return new Guid(hash).ToString();
        }
    }
}
