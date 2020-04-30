using System;

namespace Standard.Licensing.LicenseKey.KeyGeneration
{
	public class StandardLicenseFactory<T> : ILicenseFactory<T>
	{
		private readonly LicenseKey<T> _licenseKey;

		public StandardLicenseFactory()
		{
			_licenseKey = new LicenseKey<T>();
		}

		public StandardLicenseFactory(T licenseInformation)
		{
			_licenseKey = new LicenseKey<T>(licenseInformation);
		}

		public ILicenseFactory<T> WithActivationDate(DateTime activationDate)
		{
			_licenseKey.ActivationDate = activationDate.ToUniversalTime();
			return this;
		}

		public ILicenseFactory<T> WithExpirationDate(DateTime expirationDate)
		{
			_licenseKey.ExpirationDate = expirationDate.ToUniversalTime();
			return this;
		}

		public ILicenseFactory<T> WithType(LicenseType licenseType)
		{
			_licenseKey.LicenseType = licenseType;
			return this;
		}

		public ILicenseFactory<T> WithName(string licenseName)
		{
			_licenseKey.LicenseName = licenseName;
			return this;
		}

		public LicenseKey<T> CreateLicense()
			=> _licenseKey;

		public SignedLicense CreateAndSignLicense(LicenseSigningParameters parameters)
			=> _licenseKey.Sign(parameters);
	}
}