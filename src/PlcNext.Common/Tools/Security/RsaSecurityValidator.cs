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
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PlcNext.Common.Tools.IO;

namespace PlcNext.Common.Tools.Security
{
    internal class RsaSecurityValidator : ISecurityValidator
    {
        public void ValidateSignature(Stream dataStream, Stream signatureStream, Stream publicKeyStream)
        {
            RSAParameters parameters;
            RSASignature signature;
            using (XmlReader reader = XmlReader.Create(publicKeyStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RSAParameters));
                parameters = (RSAParameters)serializer.Deserialize(reader);
            }

            using (XmlReader reader = XmlReader.Create(signatureStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RSASignature));
                signature = (RSASignature)serializer.Deserialize(reader);
            }

            HashAlgorithmName name = string.IsNullOrEmpty(signature.HashAlgorithm)
                                         ? HashAlgorithmName.MD5
                                         : new HashAlgorithmName(signature.HashAlgorithm);
            HashAlgorithm hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(name.Name);
            if (hashAlgorithm == null)
            {
                throw new ArgumentException($"Unkown hash algorithm {signature.HashAlgorithm}");
            }

            RSASignaturePadding padding = RSASignaturePadding.Pkcs1;
            switch (signature.RsaPadding.ToUpperInvariant())
            {
                case "PKCS1":
                    padding = RSASignaturePadding.Pkcs1;
                    break;
                case "PSS":
                    padding = RSASignaturePadding.Pss;
                    break;
                default:
                    if (!string.IsNullOrEmpty(signature.RsaPadding))
                    {
                        throw new ArgumentException($"Rsa signature padding {signature.RsaPadding} is not known.");
                    }
                    break;
            }

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(parameters);

                byte[] data = dataStream.ReadToEnd();
                byte[] hash = hashAlgorithm.ComputeHash(data);

                // Verify the hash
                bool verified = rsa.VerifyHash(hash, signature.RawSignature, name, padding);
                if (!verified)
                {
                    throw new SignatureValidationException();
                }
            }
        }

        public void ValidateHash(Stream dataStream, byte[] hash, string hashAlgorithmName)
        {
            HashAlgorithmName name = string.IsNullOrEmpty(hashAlgorithmName)
                                         ? HashAlgorithmName.MD5
                                         : new HashAlgorithmName(hashAlgorithmName);
            HashAlgorithm hashAlgorithm = (HashAlgorithm)CryptoConfig.CreateFromName(name.Name);
            if (hashAlgorithm == null)
            {
                throw new ArgumentException($"Unknown hash algorithm {hashAlgorithmName}");
            }

            byte[] data = dataStream.ReadToEnd();
            byte[] actualHash = hashAlgorithm.ComputeHash(data);

            if (!actualHash.SequenceEqual(hash))
            {
                throw new HashValidationException();
            }
        }
    }
}
