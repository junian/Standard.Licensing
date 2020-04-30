using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Standard.Licensing.LicenseKey.Internals.Extensions;

namespace Standard.Licensing.LicenseKey
{
	public class LicenseSigningParameters : IDisposable
	{
		public byte[] PublicKey => Rsa.ExportRSAPublicKey();

		public byte[] PrivateKey
		{
			get
			{
				try
				{
					return Rsa.ExportRSAPrivateKey();
				}
				catch
				{
					return new byte[0];
				}
			}
		}

		[JsonIgnore] internal readonly RSA Rsa;


		/// <summary>
		/// generate signing credentials with a new key
		/// <param name="keySize"></param>
		/// </summary>
		public LicenseSigningParameters(int keySize = 2048)
		{
			Rsa = RSA.Create(keySize);
		}

		/// <summary>
		/// generate signing credentials with public key only
		/// </summary>
		/// <param name="publicKey"></param>
		/// <param name="keySize"></param>
		public LicenseSigningParameters(byte[] publicKey, int keySize = 2048) : this(keySize)
		{
			Rsa.ImportRSAPublicKey(publicKey, out int _);
		}

		/// <summary>
		/// generate signing credentials with both public and private key
		/// </summary>
		/// <param name="publicKey">the rsa public key</param>
		/// <param name="privateKey">the rsa private key</param>
		/// <param name="keySize"></param>
		public LicenseSigningParameters(
			byte[] publicKey,
			byte[] privateKey,
			int keySize = 2048) : this(keySize)
		{
			Rsa.ImportRSAPublicKey(publicKey, out _);
			Rsa.ImportRSAPrivateKey(privateKey, out _);
		}

		public void Export(string path)
			=> File.WriteAllText(
				path,
				GenerateParametersDto()
				   .ToJsonString());

		public Task ExportAsync(string path, CancellationToken cancellationToken = default)
			=> File.WriteAllTextAsync(
				path,
				GenerateParametersDto()
				   .ToJsonString(),
				cancellationToken);

		private LicenseParametersDto GenerateParametersDto()
			=> new LicenseParametersDto
			{
				PrivateKey = PrivateKey,
				PublicKey = PublicKey,
				KeyLength = Rsa.KeySize
			};

		public static LicenseSigningParameters Import(string path)
		{
			var parameters = File.ReadAllText(path)
			   .FromJson<LicenseParametersDto>();
			return new LicenseSigningParameters(
				parameters.PublicKey,
				parameters.PrivateKey,
				parameters.KeyLength);
		}

		public static async Task<LicenseSigningParameters> ImportAsync(
			string path,
			CancellationToken cancellationToken = default)
		{
			var fileContents = await File.ReadAllTextAsync(path, cancellationToken);
			var parameters = fileContents.FromJson<LicenseParametersDto>();
			return new LicenseSigningParameters(
				parameters.PublicKey,
				parameters.PrivateKey,
				parameters.KeyLength);
		}

		public void Dispose()
		{
			Rsa?.Dispose();
		}
	}

	internal class LicenseParametersDto
	{
		public byte[] PrivateKey { get; set; }

		public byte[] PublicKey { get; set; }

		public int KeyLength { get; set; }
	}
}