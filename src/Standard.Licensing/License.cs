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

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Security;
using Standard.Licensing.Security.Cryptography;

namespace Standard.Licensing
{
    /// <summary>
    /// A software license
    /// </summary>
    public class License
    {
        private readonly XElement xmlData;
        private readonly string signatureAlgorithm = X9ObjectIdentifiers.ECDsaWithSha512.Id;
        private static XmlReaderSettings defaultXmlReaderSettings = new XmlReaderSettings
        {
            IgnoreWhitespace = true, // Ignore unnecessary whitespace nodes
            IgnoreComments = true,   // Skip comment nodes to reduce processing
            IgnoreProcessingInstructions = true // Skip processing instructions
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class.
        /// </summary>
        internal License()
        {
            xmlData = new XElement("License");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="License"/> class
        /// with the specified content.
        /// </summary>
        /// <remarks>This constructor is only used for loading from XML.</remarks>
        /// <param name="xmlData">The initial content of this <see cref="License"/>.</param>
        internal License(XElement xmlData)
        {
            this.xmlData = xmlData;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="License"/>.
        /// </summary>
        public Guid Id
        {
            get { return new Guid(GetTag("Id") ?? Guid.Empty.ToString()); }
            set { if (!IsSigned) SetTag("Id", value.ToString()); }
        }

        /// <summary>
        /// Gets or set the <see cref="LicenseType"/> or this <see cref="License"/>.
        /// </summary>
        public LicenseType Type
        {
            get
            {
                return
                    (LicenseType)
                    Enum.Parse(typeof (LicenseType), GetTag("Type") ?? LicenseType.Trial.ToString(), false);
            }
            set { if (!IsSigned) SetTag("Type", value.ToString()); }
        }

        /// <summary>
        /// Get or sets the quantity of this license.
        /// E.g. the count of per-developer-licenses.
        /// </summary>
        public int Quantity
        {
            get { return int.Parse(GetTag("Quantity") ?? "0"); }
            set { if (!IsSigned) SetTag("Quantity", value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the product features of this <see cref="License"/>.
        /// </summary>
        public LicenseAttributes ProductFeatures
        {
            get
            {
                var xmlElement = xmlData.Element("ProductFeatures");

                if (!IsSigned && xmlElement == null)
                {
                    xmlData.Add(new XElement("ProductFeatures"));
                    xmlElement = xmlData.Element("ProductFeatures");
                }
                else if (IsSigned && xmlElement == null)
                {
                    return null;
                }

                return new LicenseAttributes(xmlElement, "Feature");
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Customer"/> of this <see cref="License"/>.
        /// </summary>
        public Customer Customer
        {
            get
            {
                var xmlElement = xmlData.Element("Customer");

                if (!IsSigned && xmlElement == null)
                {
                    xmlData.Add(new XElement("Customer"));
                    xmlElement = xmlData.Element("Customer");
                }
                else if (IsSigned && xmlElement == null)
                {
                    return null;
                }

                return new Customer(xmlElement);
            }
        }

        /// <summary>
        /// Gets or sets the additional attributes of this <see cref="License"/>.
        /// </summary>
        public LicenseAttributes AdditionalAttributes
        {
            get
            {
                var xmlElement = xmlData.Element("LicenseAttributes");

                if (!IsSigned && xmlElement == null)
                {
                    xmlData.Add(new XElement("LicenseAttributes"));
                    xmlElement = xmlData.Element("LicenseAttributes");
                }
                else if (IsSigned && xmlElement == null)
                {
                    return null;
                }
                
                return new LicenseAttributes(xmlElement, "Attribute");
            }
        }

        /// <summary>
        /// Gets or sets the expiration date of this <see cref="License"/>.
        /// Use this property to set the expiration date for a trial license
        /// or the expiration of support & subscription updates for a standard license.
        /// </summary>
        public DateTime Expiration
        {
            get
            {
                return
                    DateTime.ParseExact(
                        GetTag("Expiration") ??
                        DateTime.MaxValue.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture)
                        , "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            }
            set { if (!IsSigned) SetTag("Expiration", value.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets the digital signature of this license.
        /// </summary>
        /// <remarks>Use the <see cref="License.Sign"/> method to compute a signature.</remarks>
        public string Signature
        {
            get { return GetTag("Signature"); }
        }

        /// <summary>
        /// Compute a signature and sign this <see cref="License"/> with the provided key.
        /// </summary>
        /// <param name="privateKey">The private key in xml string format to compute the signature.</param>
        /// <param name="passPhrase">The pass phrase to decrypt the private key.</param>
        public void Sign(string privateKey, string passPhrase)
        {
            var signTag = xmlData.Element("Signature") ?? new XElement("Signature");

            try
            {
                if (signTag.Parent != null)
                    signTag.Remove();

                var privKey = KeyFactory.FromEncryptedPrivateKeyString(privateKey, passPhrase);

                var documentToSign = Encoding.UTF8.GetBytes(xmlData.ToString(SaveOptions.DisableFormatting));
                var signer = SignerUtilities.GetSigner(signatureAlgorithm);
                signer.Init(true, privKey);
                signer.BlockUpdate(documentToSign, 0, documentToSign.Length);
                var signature = signer.GenerateSignature();
                signTag.Value = Convert.ToBase64String(signature);
            }
            finally
            {
                xmlData.Add(signTag);
            }
        }

        /// <summary>
        /// Determines whether the <see cref="License.Signature"/> property verifies for the specified key.
        /// </summary>
        /// <param name="publicKey">The public key in xml string format to verify the <see cref="License.Signature"/>.</param>
        /// <returns>true if the <see cref="License.Signature"/> verifies; otherwise false.</returns>
        public bool VerifySignature(string publicKey)
        {
            var signTag = xmlData.Element("Signature");

            if (signTag == null)
                return false;

            try
            {
                signTag.Remove();

                var pubKey = KeyFactory.FromPublicKeyString(publicKey);

                var documentToSign = Encoding.UTF8.GetBytes(xmlData.ToString(SaveOptions.DisableFormatting));
                var signer = SignerUtilities.GetSigner(signatureAlgorithm);
                signer.Init(false, pubKey);
                signer.BlockUpdate(documentToSign, 0, documentToSign.Length);

                return signer.VerifySignature(Convert.FromBase64String(signTag.Value));
            }
            finally
            {
                xmlData.Add(signTag);
            }
        }

        /// <summary>
        /// Create a new <see cref="License"/> using the <see cref="ILicenseBuilder"/>
        /// fluent api.
        /// </summary>
        /// <returns>An instance of the <see cref="ILicenseBuilder"/> class.</returns>
        public static ILicenseBuilder New()
        {
            return new LicenseBuilder();
        }

        /// <summary>
        /// Loads a <see cref="License"/> from a string that contains XML.
        /// </summary>
        /// <param name="xmlString">A <see cref="string"/> that contains XML.</param>
        /// <returns>A <see cref="License"/> populated from the <see cref="string"/> that contains XML.</returns>
        public static License Load(string xmlString)
        {
            return new License(XElement.Parse(xmlString, LoadOptions.None));
        }

        /// <summary>
        /// Loads a <see cref="License"/> by using the specified <see cref="Stream"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that contains the XML.</param>
        /// <returns>A <see cref="License"/> populated from the <see cref="Stream"/> that contains XML.</returns>
        public static License Load(Stream stream)
        {
            // Use XmlReader for efficient XML parsing
            using (var reader = XmlReader.Create(stream, defaultXmlReaderSettings))
            {
                var xmlData = XElement.Load(reader);
                return new License(xmlData);
            }
        }

        /// <summary>
        /// Loads a <see cref="License"/> by using the specified <see cref="TextReader"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that contains the XML.</param>
        /// <returns>A <see cref="License"/> populated from the <see cref="TextReader"/> that contains XML.</returns>
        public static License Load(TextReader reader)
        {
            return new License(XElement.Load(reader, LoadOptions.None));
        }

        /// <summary>
        /// Loads a <see cref="License"/> by using the specified <see cref="XmlReader"/>
        /// that contains the XML.
        /// </summary>
        /// <param name="reader">A <see cref="XmlReader"/> that contains the XML.</param>
        /// <returns>A <see cref="License"/> populated from the <see cref="TextReader"/> that contains XML.</returns>
        public static License Load(XmlReader reader)
        {
            // Directly load the XML from the XmlReader
            var xmlData = XElement.Load(reader);
            return new License(xmlData);
        }

        /// <summary>
        /// Serialize this <see cref="License"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that the 
        /// <see cref="License"/> will be written to.</param>
        public void Save(Stream stream)
        {
            // Use XmlWriter for efficient XML writing
            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
            {
                xmlData.WriteTo(writer);
            }
        }
        
        /// <summary>
        /// Serialize this <see cref="License"/> to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">A <see cref="TextWriter"/> that the 
        /// <see cref="License"/> will be written to.</param>
        public void Save(TextWriter textWriter)
        {
            using (var writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true }))
            {
                xmlData.WriteTo(writer);
            }
        }

        /// <summary>
        /// Serialize this <see cref="License"/> to a <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="xmlWriter">A <see cref="XmlWriter"/> that the 
        /// <see cref="License"/> will be written to.</param>
        public void Save(XmlWriter xmlWriter)
        {
            xmlData.Save(xmlWriter);
        }

        /// <summary>
        /// Returns the indented XML for this <see cref="License"/>.
        /// </summary>
        /// <returns>A string containing the indented XML.</returns>
        public override string ToString()
        {
            return xmlData.ToString();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="License"/> is already signed.
        /// </summary>
        private bool IsSigned
        {
            get { return (!string.IsNullOrEmpty(Signature)); }
        }

        private void SetTag(string name, string value)
        {
            var element = xmlData.Element(name);

            if (element == null)
            {
                element = new XElement(name);
                xmlData.Add(element);
            }

            if (value != null)
                element.Value = value;
        }

        private string GetTag(string name)
        {
            var element = xmlData.Element(name);
            return element != null ? element.Value : null;
        }
    }
}