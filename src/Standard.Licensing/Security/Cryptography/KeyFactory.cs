//
// Copyright © 2012 - 2013 Nauck IT KG     http://www.nauck-it.de
//
// Author:
//  Daniel Nauck        <d.nauck(at)nauck-it.de>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Buffers;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

#if NET6_0_OR_GREATER
using System.Security.Cryptography; // Use native cryptography in .NET 6.0+
#endif

namespace Standard.Licensing.Security.Cryptography
{
    internal static class KeyFactory
    {
        private static readonly string keyEncryptionAlgorithm = PkcsObjectIdentifiers.PbeWithShaAnd3KeyTripleDesCbc.Id;

        /// <summary>
        /// Encrypts and encodes the private key.
        /// </summary>
        /// <param name="key">The private key.</param>
        /// <param name="passPhrase">The pass phrase to encrypt the private key.</param>
        /// <returns>The encrypted private key.</returns>
        public static string ToEncryptedPrivateKeyString(AsymmetricKeyParameter key, string passPhrase)
        {
            // Rent a buffer for the salt (16 bytes for the salt)
            var salt = ArrayPool<byte>.Shared.Rent(16);

            try
            {
#if NET6_0_OR_GREATER
                // Use built-in cryptography (e.g., RNGCryptoServiceProvider) in .NET 6.0+
                RandomNumberGenerator.Fill(salt.AsSpan(0, 16));
#else
                // Use BouncyCastle for .NET Standard 2.0 or other versions
                var secureRandom = SecureRandom.GetInstance("SHA256PRNG");
                secureRandom.SetSeed(secureRandom.GenerateSeed(16)); // Seed generation
                secureRandom.NextBytes(salt, 0, 16);
#endif
                // Encrypt the key (BouncyCastle library used for all versions)
                var encryptedKey = PrivateKeyFactory.EncryptKey(keyEncryptionAlgorithm, passPhrase.ToCharArray(), salt, 10, key);

                return Convert.ToBase64String(encryptedKey);
            }
            finally
            {
                // Return the rented buffer to the pool
                ArrayPool<byte>.Shared.Return(salt);
            }
        }

        /// <summary>
        /// Decrypts the provided private key.
        /// </summary>
        /// <param name="privateKey">The encrypted private key.</param>
        /// <param name="passPhrase">The pass phrase to decrypt the private key.</param>
        /// <returns>The private key.</returns>
        public static AsymmetricKeyParameter FromEncryptedPrivateKeyString(string privateKey, string passPhrase)
        {
            return PrivateKeyFactory.DecryptKey(passPhrase.ToCharArray(), Convert.FromBase64String(privateKey));
        }

        /// <summary>
        /// Encodes the public key into DER encoding.
        /// </summary>
        /// <param name="key">The public key.</param>
        /// <returns>The encoded public key.</returns>
        public static string ToPublicKeyString(AsymmetricKeyParameter key)
        {
            return
                Convert.ToBase64String(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key)
                                               .ToAsn1Object()
                                               .GetDerEncoded());
        }

        /// <summary>
        /// Decoded the public key from DER encoding.
        /// </summary>
        /// <param name="publicKey">The encoded public key.</param>
        /// <returns>The public key.</returns>
        public static AsymmetricKeyParameter FromPublicKeyString(string publicKey)
        {
            return PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
        }
    }
}