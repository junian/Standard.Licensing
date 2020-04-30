using System.Security.Cryptography;

using Standard.Licensing.LicenseKey.TestLicenses;

using Xunit;

namespace Standard.Licensing.LicenseKey
{
	public class SignedLicenseTests
	{
		[Fact]
		public void TestSignatureValid_GivenValidSignature_Succeeds()
		{
			var token = LicenseKeyManager.Create(new SimpleTestLicense())
			   .CreateAndSignLicense(new LicenseSigningParameters());

			Assert.True(token.SignatureValid());
		}

		[Fact]
		public void TestSignatureValid_GivenInvalidSignature_Fails()
		{
			var token = LicenseKeyManager.Create(new SimpleTestLicense())
			   .CreateAndSignLicense(new LicenseSigningParameters());

			var newSignature = new byte[2048 / 8];
			RandomNumberGenerator.Fill(newSignature);

			token = new SignedLicense(token.LicenseData, newSignature);
			
			Assert.False(token.SignatureValid());
		}
	}
}
