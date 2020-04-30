using System;

namespace Standard.Licensing.LicenseKey.KeyGeneration
{
	public interface ILicenseFactory<T>
	{
		/// <summary>
		/// configure the activation date with the certain value
		/// </summary>
		/// <param name="activationDate">the activation date the license should have</param>
		/// <returns></returns>
		ILicenseFactory<T> WithActivationDate(DateTime activationDate);

		/// <summary>
		/// configure the expiration date the license should have
		/// </summary>
		/// <param name="expirationDate">the expiration date the license should have</param>
		/// <returns></returns>
		ILicenseFactory<T> WithExpirationDate(DateTime expirationDate);

		/// <summary>
		/// configure the license type the license should have
		/// </summary>
		/// <param name="licenseType">the type of license this should be</param>
		/// <returns></returns>
		ILicenseFactory<T> WithType(LicenseType licenseType);

		/// <summary>
		/// configure the license name that the license should have
		/// </summary>
		/// <param name="licenseName">the name that the license should have</param>
		/// <returns></returns>
		ILicenseFactory<T> WithName(string licenseName);

		/// <summary>
		/// creates the license as an object without signing 
		/// </summary>
		/// <returns></returns>
		LicenseKey<T> CreateLicense();

		/// <summary>
		/// run an operation to create the signed, binary version of the license
		/// </summary>
		/// <param name="parameters">the signing parameters the license should have</param>
		/// <returns></returns>
		SignedLicense CreateAndSignLicense(LicenseSigningParameters parameters);

	}
}