using Standard.Licensing.LicenseKey.TestLicenses;

using Xunit;

namespace Standard.Licensing.LicenseKey
{
	public class TestLicenseKeyManager
	{
		[Fact]
		public void TestCanCreateFactory_GivenNoModel()
			=> Assert.NotNull(LicenseKeyManager.Create());

		[Fact]
		public void TestCanCreateFactory_GiveLicenseModel()
			=> Assert.NotNull(LicenseKeyManager.Create(new SimpleTestLicense()));

		[Fact]
		public void TestCanLoadSignedLicense_GivenSignedLicense()
			=> Assert.NotNull(
				LicenseKeyManager.Load(
					LicenseKeyManager.Create()
					   .CreateAndSignLicense(new LicenseSigningParameters())));

		[Fact]
		public void TestCanLoadLicense_GivenLicense()
			=> Assert.NotNull(LicenseKeyManager.Load(new LicenseKey<object>()));

		[Fact]
		public void TestCanLoadSignedLicense_GivenBlankLicense()
			=> Assert.NotNull(
				LicenseKeyManager.Load<SimpleTestLicense>(
					LicenseKeyManager.Create(new SimpleTestLicense())
					   .CreateAndSignLicense(new LicenseSigningParameters())));

		[Fact]
		public void TestCanLoadLicense_GivenBlankGenericLicense()
			=> Assert.NotNull(LicenseKeyManager.Load(new LicenseKey<SimpleTestLicense>()));

	}
}