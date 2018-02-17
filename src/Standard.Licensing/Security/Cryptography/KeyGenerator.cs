﻿//
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

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;

namespace Portable.Licensing.Security.Cryptography
{
    /// <summary>
    /// Represents a generator for signature keys of <see cref="License"/>.
    /// </summary>
    public class KeyGenerator
    {
        private readonly IAsymmetricCipherKeyPairGenerator keyGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGenerator"/> class
        /// with a key size of 256 bits.
        /// </summary>
        public KeyGenerator()
            : this(256)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGenerator"/> class
        /// with the specified key size.
        /// </summary>
        /// <remarks>Following key sizes are supported:
        /// - 192
        /// - 224
        /// - 239
        /// - 256 (default)
        /// - 384
        /// - 521</remarks>
        /// <param name="keySize">The key size.</param>
        public KeyGenerator(int keySize)
            : this(keySize, SecureRandom.GetSeed(32))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGenerator"/> class
        /// with the specified key size and seed.
        /// </summary>
        /// <remarks>Following key sizes are supported:
        /// - 192
        /// - 224
        /// - 239
        /// - 256 (default)
        /// - 384
        /// - 521</remarks>
        /// <param name="keySize">The key size.</param>
        /// <param name="seed">The seed.</param>
        public KeyGenerator(int keySize, byte[] seed)
        {
            var secureRandom = SecureRandom.GetInstance("SHA256PRNG");
            secureRandom.SetSeed(seed);

            var keyParams = new KeyGenerationParameters(secureRandom, keySize);
            keyGenerator = new ECKeyPairGenerator();
            keyGenerator.Init(keyParams);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="KeyGenerator"/> class.
        /// </summary>
        public static KeyGenerator Create()
        {
            return new KeyGenerator();
        }

        /// <summary>
        /// Generates a private/public key pair for license signing.
        /// </summary>
        /// <returns>An <see cref="KeyPair"/> containing the keys.</returns>
        public KeyPair GenerateKeyPair()
        {
            return new KeyPair(keyGenerator.GenerateKeyPair());
        }
    }
}