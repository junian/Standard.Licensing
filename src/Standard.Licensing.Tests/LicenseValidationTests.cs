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

using NUnit.Framework;

using Standard.Licensing.Security.Cryptography;
using Standard.Licensing.Validation;

namespace Standard.Licensing.Tests
{
    [TestFixture]
    public class LicenseValidationTests
    {
        [Test]
        public void Can_Validate_Valid_Signature()
        {
            const string? publicKey = "MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            const string? licenseData = @"<License>
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

            License? license = License.Load(licenseData);

            IEnumerable<IValidationFailure>? validationResults = license
                                                                 .Validate()
                                                                 .Signature(publicKey)
                                                                 .AssertValidLicense();

            IEnumerable<IValidationFailure> validationFailures = validationResults.ToList();
            Assert.That(validationFailures, Is.Not.Null);
            Assert.That(validationFailures.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Can_Validate_Invalid_Signature()
        {
            const string publicKey = "MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            const string licenseData = @"<License>
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

            License? license = License.Load(licenseData);

            List<IValidationFailure> validationResults = license
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
            const string licenseData = @"<License>
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

            License? license = License.Load(licenseData);

            List<IValidationFailure> validationResults = license
                                                         .Validate()
                                                         .ExpirationDate()
                                                         .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<LicenseExpiredValidationFailure>());

        }

        [Test]
        public void Can_Validate_Expired_ExpirationDate_CustomDateTime()
        {
            const string licenseData = @"<License>
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

            License? license = License.Load(licenseData);

            List<IValidationFailure> validationResults = license
                                                         .Validate()
                                                         .ExpirationDate(systemDateTime: new DateTime(1900, 1, 2, 0, 0, 0, DateTimeKind.Utc))
                                                         .AssertValidLicense().ToList();

            Assert.That(validationResults, Is.Not.Null);
            Assert.That(validationResults.Count(), Is.EqualTo(1));
            Assert.That(validationResults.FirstOrDefault(), Is.TypeOf<LicenseExpiredValidationFailure>());

        }

        [Test]
        public void Can_Validate_CustomAssertion()
        {
            string publicKey = "MIIBKjCB4wYHKoZIzj0CATCB1wIBATAsBgcqhkjOPQEBAiEA/////wAAAAEAAAAAAAAAAAAAAAD///////////////8wWwQg/////wAAAAEAAAAAAAAAAAAAAAD///////////////wEIFrGNdiqOpPns+u9VXaYhrxlHQawzFOw9jvOPD4n0mBLAxUAxJ02CIbnBJNqZnjhE50mt4GffpAEIQNrF9Hy4SxCR/i85uVjpEDydwN9gS3rM6D0oTlF2JjClgIhAP////8AAAAA//////////+85vqtpxeehPO5ysL8YyVRAgEBA0IABNVLQ1xKY80BFMgGXec++Vw7n8vvNrq32PaHuBiYMm0PEj2JoB7qSSWhfgcjxNVJsxqJ6gDQVWgl0r7LH4dr0KU=";
            string licenseData = @"<License>
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

            License? license = License.Load(licenseData);

            List<IValidationFailure> validationResults = license
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
            const string publicKey = "1234";
            const string licenseData = "<license expiration='2013-06-30T00:00:00.0000000' type='Trial'><name>John Doe</name></license>";

            License? license = License.Load(licenseData);

            List<IValidationFailure> validationResults = license
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
            KeyGenerator? keyGenerator = KeyGenerator.Create();
            KeyPair? keyPair = keyGenerator.GenerateKeyPair();
            string? publicKey = keyPair.ToPublicKeyString();

            const string invalidLicense = @"<License>
  <Signature>WFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFg=</Signature>
</License>";

            License? licenseToVerify = License.Load(invalidLicense);

            IEnumerable<IValidationFailure>? validationFailures = licenseToVerify
                                                                  .Validate()
                                                                  .Signature(publicKey)
                                                                  .AssertValidLicense();

            int count = 0;
            IEnumerable<IValidationFailure> enumerable = validationFailures.ToList();
            foreach (IValidationFailure? v in enumerable)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(1));
            Assert.That(enumerable.ToArray().Length, Is.EqualTo(1));
            Assert.That(enumerable.ToArray().Length, Is.EqualTo(1));
        }
    }
}