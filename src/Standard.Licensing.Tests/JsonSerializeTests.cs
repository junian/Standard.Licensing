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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using NUnit.Framework;

namespace Standard.Licensing.Tests
{
    [TestFixture]
    public class JsonSerializeTests
    {
        private string passPhrase;
        private string privateKey;
        private string publicKey;

        [SetUp]
        public void Init()
        {
            passPhrase = Guid.NewGuid().ToString();
            var keyGenerator = Security.Cryptography.KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            privateKey = keyPair.ToEncryptedPrivateKeyString(passPhrase);
            publicKey = keyPair.ToPublicKeyString();
        }

        private static DateTime ConvertToRfc1123(DateTime dateTime)
        {
            return DateTime.ParseExact(
                dateTime.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture)
                , "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }


        [Test]
        public void Can_Generate_And_Validate_Signature_With_Empty_License()
        {
            var license = License.New()
                                 .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            Assert.That(license, Is.Not.Null);
            Assert.That(license.Signature, Is.Not.Null);

            // validate json
            var jsonText = license.ToJson();
            Assert.That(jsonText, Is.Not.Empty);



            // validate default values when not set
            Assert.That(license.Id, Is.EqualTo(Guid.Empty));
            Assert.That(license.Type, Is.EqualTo(LicenseType.Trial));
            Assert.That(license.Quantity, Is.EqualTo(0));
            Assert.That(license.ProductFeatures, Is.Null);
            Assert.That(license.Customer, Is.Null);
            Assert.That(license.Expiration, Is.EqualTo(ConvertToRfc1123(DateTime.MaxValue)));

            // verify signature
            Assert.That(license.VerifySignature(publicKey), Is.True);
        }

        [Test]
        public void Can_Generate_And_Validate_Signature_With_Standard_License()
        {
            var licenseId = Guid.NewGuid();
            var customerName = "Max Mustermann";
            var customerEmail = "max@mustermann.tld";
            var expirationDate = DateTime.Now.AddYears(1);
            var productFeatures = new Dictionary<string, string>
                                      {
                                          {"Sales Module", "yes"},
                                          {"Purchase Module", "yes"},
                                          {"Maximum Transactions", "10000"}
                                      };

            var additionalAttributes = new Dictionary<string, string>
                                      {
                                          {"Domain", "test.com"},
                                          {"Host", "172.17.0.1"}
                                      };

            var license = License.New()
                                 .WithUniqueIdentifier(licenseId)
                                 .As(LicenseType.Standard)
                                 .WithMaximumUtilization(10)
                                 .WithProductFeatures(productFeatures)
                                 .WithAdditionalAttributes(additionalAttributes)
                                 .LicensedTo(customerName, customerEmail)
                                 .ExpiresAt(expirationDate)
                                 .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            Assert.That(license, Is.Not.Null);
            Assert.That(license.Signature, Is.Not.Null);


            // validate json
            var jsonText = license.ToJson();
            Assert.That(jsonText, Is.Not.Empty);

            var parsedLicense = jsonText.ToLicense();
            Assert.That(parsedLicense, Is.Not.Null);


            // validate default values when not set
            Assert.That(parsedLicense.Id, Is.EqualTo(licenseId));
            Assert.That(parsedLicense.Type, Is.EqualTo(LicenseType.Standard));
            Assert.That(parsedLicense.Quantity, Is.EqualTo(10));
            Assert.That(parsedLicense.ProductFeatures, Is.Not.Null);
            Assert.That(parsedLicense.ProductFeatures.GetAll(), Is.EquivalentTo(productFeatures));
            Assert.That(parsedLicense.Customer, Is.Not.Null);
            Assert.That(parsedLicense.Customer.Name, Is.EqualTo(customerName));
            Assert.That(parsedLicense.Customer.Email, Is.EqualTo(customerEmail));
            Assert.That(parsedLicense.Expiration, Is.EqualTo(ConvertToRfc1123(expirationDate)));

            // verify signature
            Assert.That(parsedLicense.VerifySignature(publicKey), Is.True);
        }



    }
}