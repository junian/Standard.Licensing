//
// Copyright (c) 2012 - 2013 Nauck IT KG        http://www.nauck-it.de
// Copyright (c) 2018 - 2024 Junian Triajianto	https://www.junian.dev
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Standard.Licensing.Validation;

namespace Standard.Licensing.Tests
{
    [TestFixture]
    public class LicenseValidationTests
    {
        [Test]
        public void Can_Validate_Valid_Signature()
        {
            var publicKey =
                @"MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Sun, 31 Dec 1899 23:00:00 GMT</Expiration>
                                  <Quantity>1</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            var validationResults = license
                .Validate()
                .Signature(publicKey)
                .AssertValidLicense();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Can_Validate_Invalid_Signature()
        {
            var publicKey =
                @"MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Sun, 31 Dec 1899 23:00:00 GMT</Expiration>
                                  <Quantity>999</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            var validationResults = license
                .Validate()
                .Signature(publicKey)
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<InvalidSignatureValidationFailure>());
        }

        [Test]
        public void Can_Validate_Expired_ExpirationDate()
        {
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Sun, 31 Dec 1899 23:00:00 GMT</Expiration>
                                  <Quantity>1</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            Assert.That(license.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));

            var validationResults = license
                .Validate()
                .ExpirationDate()
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<LicenseExpiredValidationFailure>());
        }

        [Test]
        public void Can_Validate_NotExpired_ExpirationDate()
        {
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Fri, 31 Dec 2100 23:00:00 GMT</Expiration>
                                  <Quantity>1</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            Assert.That(license.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));

            var validationResults = license
                .Validate()
                .ExpirationDate()
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(0));
        }

        public static IEnumerable<TestCaseData> LocalAndUtcExpired
        {
            get
            {
                yield return new TestCaseData(new DateTime(1899, 12, 31, 23, 01, 0, DateTimeKind.Utc));
                yield return new TestCaseData(new DateTime(1899, 12, 31, 23, 01, 0, DateTimeKind.Utc).ToLocalTime());
                yield return new TestCaseData(new DateTime(1899, 12, 31, 23, 30, 0, DateTimeKind.Utc));
                yield return new TestCaseData(new DateTime(1899, 12, 31, 23, 30, 0, DateTimeKind.Utc).ToLocalTime());
            }
        }

        [Test, TestCaseSource(nameof(LocalAndUtcExpired))]
        public void Can_Validate_Expired_ExpirationDate_CustomDateTime(DateTime expirationDate)
        {
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Sun, 31 Dec 1899 23:00:00 GMT</Expiration>
                                  <Quantity>1</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            Assert.That(license.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));

            var validationResults = license
                .Validate()
                .ExpirationDate(systemDateTime: expirationDate)
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<LicenseExpiredValidationFailure>());
        }

        public static IEnumerable<TestCaseData> LocalAndUtcNotExpired
        {
            get
            {
                yield return new TestCaseData(new DateTime(1899, 12, 31, 22, 59, 0, DateTimeKind.Utc));
                yield return new TestCaseData(new DateTime(1899, 12, 31, 22, 59, 0, DateTimeKind.Utc).ToLocalTime());
                yield return new TestCaseData(new DateTime(1899, 12, 31, 22, 30, 0, DateTimeKind.Utc));
                yield return new TestCaseData(new DateTime(1899, 12, 31, 22, 30, 0, DateTimeKind.Utc).ToLocalTime());
            }
        }

        [Test, TestCaseSource(nameof(LocalAndUtcNotExpired))]
        public void Can_Validate_NotExpired_ExpirationDate_CustomDateTime(DateTime expirationDate)
        {
            var licenseData = @"<License>
                                  <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                                  <Type>Trial</Type>
                                  <Expiration>Sun, 31 Dec 1899 23:00:00 GMT</Expiration>
                                  <Quantity>1</Quantity>
                                  <Customer>
                                    <Name>John Doe</Name>
                                    <Email>john@doe.tld</Email>
                                  </Customer>
                                  <LicenseAttributes />
                                  <ProductFeatures />
                                  <Signature>MEUCIQCCEDAldOZHHIKvYZRDdzUP4V51y23d6deeK5jIFy27GQIgDz2CndjBh4Vb8tiC3FGQ6fn3GKt8d/P5+luJH0cWv+I=</Signature>
                                </License>";

            var license = License.Load(licenseData);

            Assert.That(license.Expiration.Kind, Is.EqualTo(DateTimeKind.Utc));

            var validationResults = license
                .Validate()
                .ExpirationDate(systemDateTime: expirationDate)
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Can_Validate_CustomAssertion()
        {
            var publicKey = @"MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            var licenseData = @"<License>
                              <Id>77d4c193-6088-4c64-9663-ed7398ae8c1a</Id>
                              <Type>Trial</Type>
                              <Expiration>Thu, 31 Dec 2009 23:00:00 GMT</Expiration>
                              <Quantity>1</Quantity>
                              <Customer>
                                <Name>John Doe</Name>
                                <Email>john@doe.tld</Email>
                              </Customer>
                              <LicenseAttributes>
                                <Attribute name=""Assembly Signature"">123456789</Attribute>
                              </LicenseAttributes>
                              <ProductFeatures>
                                <Feature name=""Sales Module"">yes</Feature>
                                <Feature name=""Workflow Module"">yes</Feature>
                                <Feature name=""Maximum Transactions"">10000</Feature>
                              </ProductFeatures>
                              <Signature>MEUCIQCa6A7Cts5ex4rGHAPxiXpy+2ocZzTDSP7SsddopKUx5QIgHnqv0DjoOpc+K9wALqajxxvmLCRJAywCX5vDAjmWqr8=</Signature>
                            </License>";

            var license = License.Load(licenseData);

            var validationResults = license
                .Validate()
                .AssertThat(lic => lic.ProductFeatures.Contains("Sales Module"),
                            new GeneralValidationFailure {Message = "Sales Module not licensed!"})
                .And()
                .AssertThat(lic => lic.AdditionalAttributes.Get("Assembly Signature") == "123456789",
                            new GeneralValidationFailure {Message = "Assembly Signature does not match!"})
                .And()
                .Signature(publicKey)
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Do_Not_Crash_On_Invalid_Data()
        {
            var publicKey = "1234";
            var licenseData =
                @"<license expiration='2013-06-30T00:00:00.0000000' type='Trial'><name>John Doe</name></license>";

            var license = License.Load(licenseData);

            var validationResults = license
                .Validate()
                .ExpirationDate()
                .And()
                .Signature(publicKey)
                .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<InvalidSignatureValidationFailure>());

        }

        [Test]
        public void Test_ValidationChainBuilder_ValidationFailure_List()
        {
            var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            var publicKey = keyPair.ToPublicKeyString();

            var invalidLicense = @"<License>
  <Signature>WFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFg=</Signature>
</License>";

            var licenseToVerify = License.Load(invalidLicense);

            var validationFailures = licenseToVerify
                .Validate()
                .Signature(publicKey)
                .AssertValidLicense();

            var count = 0;
            foreach (var v in validationFailures)
                count++;

            Assert.That(count, Is.EqualTo(1));
            Assert.That(validationFailures.ToArray().Length, Is.EqualTo(1));
            Assert.That(validationFailures.ToArray().Length, Is.EqualTo(1));
        }
    }
}