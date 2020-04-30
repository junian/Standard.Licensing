using System;

using Standard.Licensing.LicenseKey.TestLicenses;

using Xunit;

namespace Standard.Licensing.LicenseKey.KeyGeneration
{
	public class StandardLicenseFactoryTests
	{
		private readonly ILicenseFactory<SimpleTestLicense> _licenseFactory;

		public StandardLicenseFactoryTests()
		{
			_licenseFactory =
				new StandardLicenseFactory<SimpleTestLicense>(
					SimpleTestLicense.CreateDefaultTestLicense());
		}

		[Fact]
		public void Set_ActivationDate_DateSet()
		{
			var date = DateTime.UtcNow;
			_licenseFactory.WithActivationDate(date);

			Assert.Equal(
				date,
				_licenseFactory.CreateLicense()
				   .ActivationDate);
		}

		[Fact]
		public void Set_ExpirationDate_DateSet()
		{
			var date = DateTime.UtcNow;
			_licenseFactory.WithExpirationDate(date);

			Assert.Equal(
				date,
				_licenseFactory.CreateLicense()
				   .ExpirationDate);
		}

		[Fact]
		public void Set_Type_TypeSet()
		{
			_licenseFactory.WithType(LicenseType.Free);

			Assert.Equal(
				LicenseType.Free,
				_licenseFactory.CreateLicense()
				   .LicenseType);
		}

		[Fact]
		public void Set_Name_NamSet()
		{
			const string name = "some name";

			_licenseFactory.WithName(name);
			Assert.Equal(
				name,
				_licenseFactory.CreateLicense()
				   .LicenseName);
		}

		[Fact]
		public void CreateLicense_ReturnsLicense()
		{
			Assert.NotNull(_licenseFactory.CreateLicense());
		}

		[Fact]
		public void CreateAndSignLicense_ReturnsValidSignature()
		{
			using var signingParameters = new LicenseSigningParameters();

			Assert.NotNull(_licenseFactory.CreateAndSignLicense(signingParameters));
		}

	}
}
