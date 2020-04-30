using Standard.Licensing.LicenseKey.KeyGeneration;
using Standard.Licensing.LicenseKey.KeyValidation;

namespace Standard.Licensing.LicenseKey
{
	public static class LicenseKeyManager
	{

		public static ILicenseFactory<object> Create()
			=> new StandardLicenseFactory<object>(new LicenseKey<object>());

		public static ILicenseFactory<T> Create<T>(T licenseData)
			=> new StandardLicenseFactory<T>(licenseData);

		public static ILicenseValidator<object> Load(SignedLicense signedLicense)
			=> Load<object>(signedLicense);

		public static ILicenseValidator<object> Load(LicenseKey<object> license)
			=> new StandardLicenseValidator<object>(license);

		public static ILicenseValidator<T> Load<T>(SignedLicense signedLicense)
			=> new StandardLicenseValidator<T>(signedLicense);

		public static ILicenseValidator<T> Load<T>(LicenseKey<T> licenseKey)
			=> new StandardLicenseValidator<T>(licenseKey);

	}
}
