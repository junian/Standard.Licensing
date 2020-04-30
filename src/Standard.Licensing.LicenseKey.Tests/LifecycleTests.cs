using System;
using System.IO;
using System.Threading.Tasks;

using Standard.Licensing.LicenseKey.TestLicenses;

using Xunit;

namespace Standard.Licensing.LicenseKey
{
	public class LifecycleTests
	{

		[Fact]
		public void Test_GenerateAndReadLicense_Normal_Succeeds()
		{
			using var parameters = new LicenseSigningParameters();

			var license = LicenseKeyManager.Create(new SimpleTestLicense())
			   .CreateAndSignLicense(parameters);

			var validationResults = LicenseKeyManager.Load<SimpleTestLicense>(license)
			   .Validate();

			Assert.False(validationResults.HasErrors);
		}

		[Fact]
		public void Test_GenerateAndReadLicense_BadSigningParameters_Fails()
		{
			using var originalParameters = new LicenseSigningParameters();

			var license = LicenseKeyManager.Create(new SimpleTestLicense())
			   .CreateAndSignLicense(originalParameters);

			using var newParameters = new LicenseSigningParameters();
			license = new SignedLicense(license.LicenseData, newParameters.PublicKey);

			Assert.Throws<InvalidOperationException>(
				() => LicenseKeyManager.Load<SimpleTestLicense>(license)
				   .Validate());
		}

		[Fact]
		public void Test_GenerateAndReadLicense_ExportAndImport_Succeeds()
		{
			try
			{
				using var parameters = new LicenseSigningParameters();

				parameters.Export("lic.dat");
				using var importedParameters = LicenseSigningParameters.Import("lic.dat");

				var license = LicenseKeyManager.Create(new SimpleTestLicense())
				   .CreateAndSignLicense(importedParameters);

				var results = LicenseKeyManager.Load<SimpleTestLicense>(license)
				   .Validate();

				Assert.False(results.HasErrors);
			}
			finally
			{
				File.Delete("lic.dat");
			}
		}

		[Fact]
		public async Task Test_GenerateAndReadLicenseAsync_ExportAndImport_Succeeds()
		{
			try
			{
				using var parameters = new LicenseSigningParameters();

				await parameters.ExportAsync("lic.dat");
				using var importedParameters =
					await LicenseSigningParameters.ImportAsync("lic.dat");

				var license = LicenseKeyManager.Create(new SimpleTestLicense())
				   .CreateAndSignLicense(importedParameters);

				var results = LicenseKeyManager.Load<SimpleTestLicense>(license)
				   .Validate();

				Assert.False(results.HasErrors);
			}
			finally
			{
				File.Delete("lic.dat");
			}
		}
	}
}
