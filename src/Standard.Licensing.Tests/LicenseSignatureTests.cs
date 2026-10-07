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
    public class LicenseSignatureTests
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
                , "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        }

        private static DateTime NormalizeToUtcMidnight(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        }

        [Test]
        public void Can_Generate_And_Validate_Signature_With_Empty_License()
        {
            var license = License.New()
                                 .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            Assert.That(license, Is.Not.Null);
            Assert.That(license.Signature, Is.Not.Null);

            // validate xml
            var xmlElement = XElement.Parse(license.ToString(), LoadOptions.None);
            Assert.That(xmlElement.HasElements, Is.True);

            // validate default values when not set
            Assert.That(license.Id, Is.EqualTo(Guid.Empty));
            Assert.That(license.Type, Is.EqualTo(LicenseType.Trial));
            Assert.That(license.Quantity, Is.EqualTo(0));
            Assert.That(license.ProductFeatures, Is.Null);
            Assert.That(license.Customer, Is.Null);
            Assert.That(license.Expiration, Is.EqualTo(ConvertToRfc1123(DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc))));

            // verify signature
            Assert.That(license.VerifySignature(publicKey), Is.True);
        }

        public static IEnumerable<TestCaseData> LocalAndUtc
        {
            get
            {
                yield return new TestCaseData(DateTime.Now.AddMinutes(1));
                yield return new TestCaseData(DateTime.Now.AddYears(1));
                yield return new TestCaseData(DateTime.UtcNow.AddMinutes(1));
                yield return new TestCaseData(DateTime.UtcNow.AddYears(1));
                yield return new TestCaseData(DateTime.SpecifyKind(DateTime.Now.AddMinutes(1), DateTimeKind.Unspecified));
                yield return new TestCaseData(DateTime.SpecifyKind(DateTime.Now.AddYears(1), DateTimeKind.Unspecified));
            }
        }

        [Test, TestCaseSource(nameof(LocalAndUtc))]
        public void Can_Generate_And_Validate_Signature_With_Standard_License(DateTime expirationDate)
        {
            var licenseId = Guid.NewGuid();
            var customerName = "Max Mustermann";
            var customerEmail = "max@mustermann.tld";
            var productFeatures = new Dictionary<string, string>
                                      {
                                          {"Sales Module", "yes"},
                                          {"Purchase Module", "yes"},
                                          {"Maximum Transactions", "10000"}
                                      };

            var license = License.New()
                                 .WithUniqueIdentifier(licenseId)
                                 .As(LicenseType.Standard)
                                 .WithMaximumUtilization(10)
                                 .WithProductFeatures(productFeatures)
                                 .LicensedTo(customerName, customerEmail)
                                 .ExpiresAt(expirationDate)
                                 .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            Assert.That(license, Is.Not.Null);
            Assert.That(license.Signature, Is.Not.Null);

            // validate xml
            var xmlElement = XElement.Parse(license.ToString(), LoadOptions.None);
            Assert.That(xmlElement.HasElements, Is.True);

            // validate default values when not set
            Assert.That(license.Id, Is.EqualTo(licenseId));
            Assert.That(license.Type, Is.EqualTo(LicenseType.Standard));
            Assert.That(license.Quantity, Is.EqualTo(10));
            Assert.That(license.ProductFeatures, Is.Not.Null);
            Assert.That(license.ProductFeatures.GetAll(), Is.EquivalentTo(productFeatures));
            Assert.That(license.Customer, Is.Not.Null);
            Assert.That(license.Customer.Name, Is.EqualTo(customerName));
            Assert.That(license.Customer.Email, Is.EqualTo(customerEmail));
            Assert.That(license.Expiration, Is.EqualTo(NormalizeToUtcMidnight(expirationDate)));

            // verify signature
            Assert.That(license.VerifySignature(publicKey), Is.True);
        }

        [Test]
        public void Can_Detect_Hacked_License()
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

            var license = License.New()
                                 .WithUniqueIdentifier(licenseId)
                                 .As(LicenseType.Standard)
                                 .WithMaximumUtilization(10)
                                 .WithProductFeatures(productFeatures)
                                 .LicensedTo(customerName, customerEmail)
                                 .ExpiresAt(expirationDate)
                                 .CreateAndSignWithPrivateKey(privateKey, passPhrase);

            Assert.That(license, Is.Not.Null);
            Assert.That(license.Signature, Is.Not.Null);

            // verify signature
            Assert.That(license.VerifySignature(publicKey), Is.True);

            // validate xml
            var xmlElement = XElement.Parse(license.ToString(), LoadOptions.None);
            Assert.That(xmlElement.HasElements, Is.True);

            // manipulate xml
            Assert.That(xmlElement.Element("Quantity"), Is.Not.Null);
            xmlElement.Element("Quantity").Value = "11"; // now we want to have 11 licenses

            // load license from manipulated xml
            var hackedLicense = License.Load(xmlElement.ToString());

            // validate default values when not set
            Assert.That(hackedLicense.Id, Is.EqualTo(licenseId));
            Assert.That(hackedLicense.Type, Is.EqualTo(LicenseType.Standard));
            Assert.That(hackedLicense.Quantity, Is.EqualTo(11)); // now with 10+1 licenses
            Assert.That(hackedLicense.ProductFeatures, Is.Not.Null);
            Assert.That(hackedLicense.ProductFeatures.GetAll(), Is.EquivalentTo(productFeatures));
            Assert.That(hackedLicense.Customer, Is.Not.Null);
            Assert.That(hackedLicense.Customer.Name, Is.EqualTo(customerName));
            Assert.That(hackedLicense.Customer.Email, Is.EqualTo(customerEmail));
            Assert.That(hackedLicense.Expiration, Is.EqualTo(NormalizeToUtcMidnight(expirationDate)));

            // verify signature
            Assert.That(hackedLicense.VerifySignature(publicKey), Is.False);
        }

        public static IEnumerable<TestCaseData> AllDateTimeKinds
        {
            get
            {
                var date = new DateTime(2030, 6, 15, 14, 30, 0);
                yield return new TestCaseData(DateTime.SpecifyKind(date, DateTimeKind.Utc)).SetName("Utc");
                yield return new TestCaseData(DateTime.SpecifyKind(date, DateTimeKind.Local)).SetName("Local");
                yield return new TestCaseData(DateTime.SpecifyKind(date, DateTimeKind.Unspecified)).SetName("Unspecified");
            }
        }

        [Test, TestCaseSource(nameof(AllDateTimeKinds))]
        public void Expiration_NormalizesToUtcMidnight(DateTime expirationDate)
        {
            var expectedUtcMidnight = NormalizeToUtcMidnight(expirationDate);

            var licenseViaBuilder = License.New()
                .ExpiresAt(expirationDate)
                .CreateAndSignWithPrivateKey(privateKey, passPhrase);
            Assert.That(licenseViaBuilder.Expiration, Is.EqualTo(expectedUtcMidnight));
            Assert.That(licenseViaBuilder.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));

            var licenseViaProperty = License.Load("<License></License>");
            licenseViaProperty.Expiration = expirationDate;
            Assert.That(licenseViaProperty.Expiration, Is.EqualTo(expectedUtcMidnight));
            Assert.That(licenseViaProperty.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));
        }
    }
}