using Standard.Licensing.LicenseKey.Jwt;

namespace Standard.Licensing.LicenseKey
{
	public class SignedLicense
	{
		public string LicenseData { get; }
		public byte[] PublicKey { get; }

		public SignedLicense(string licenseData, byte[] publicKey)
		{
			LicenseData = licenseData;
			PublicKey = publicKey;
		}

		/// <summary>
		/// validate that the token has a valid signature
		/// </summary>
		/// <returns>true if the signature is valid, false otherwise</returns>
		public bool SignatureValid()
		{
			try
			{
				var tokenManager = new JwtTokenManager(new LicenseSigningParameters(PublicKey));
				tokenManager.ParseSecurityToken<object>(LicenseData);
				return true;
			}
			catch
			{
				return false;
			}
		}

	}
}
