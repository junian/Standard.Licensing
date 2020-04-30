using System;
using System.Collections.Generic;

using Standard.Licensing.LicenseKey.Jwt;

namespace Standard.Licensing.LicenseKey.KeyValidation
{
	public sealed class StandardLicenseValidator<T> : ILicenseValidator<T>
	{
		private readonly LicenseKey<T> _licenseKey;

		private readonly List<string> _errors = new List<string>();

		public StandardLicenseValidator(SignedLicense license)
			=> _licenseKey =
				new JwtTokenManager(new LicenseSigningParameters(license.PublicKey))
				   .ParseSecurityToken<T>(license.LicenseData);

		public StandardLicenseValidator(LicenseKey<T> licenseKey)
			=> _licenseKey = licenseKey;

		public ILicenseValidator<T> ActivatesBefore(
			DateTime dateTime,
			string message = "")
		{
			if (_licenseKey.ActivationDate <= dateTime.ToUniversalTime())
				return this;
			message = string.IsNullOrWhiteSpace(message)
				? $"Activation date {_licenseKey.ActivationDate} after {dateTime}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfActivatesBefore(DateTime dateTime, Action<DateTime> action)
		{
			if (_licenseKey.ActivationDate <= dateTime.ToUniversalTime())
				action(_licenseKey.ActivationDate);
			return this;
		}

		public ILicenseValidator<T> ActivatesAfter(
			DateTime dateTime,
			string message = "")
		{
			if (_licenseKey.ActivationDate >= dateTime.ToUniversalTime())
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"activation date {_licenseKey.ActivationDate} before {dateTime}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfActivatesAfter(DateTime dateTime, Action<DateTime> action)
		{
			if (_licenseKey.ExpirationDate >= dateTime.ToUniversalTime())
				action(_licenseKey.ActivationDate);
			return this;
		}

		public ILicenseValidator<T> ExpiresBefore(
			DateTime dateTime,
			string message = "")
		{
			if (_licenseKey.ExpirationDate <= dateTime.ToUniversalTime())
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"expiration date {_licenseKey.ExpirationDate} after {dateTime}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfExpiresBefore(DateTime dateTime, Action<DateTime> action)
		{
			if (_licenseKey.ExpirationDate <= dateTime.ToUniversalTime())
				action(_licenseKey.ActivationDate);
			return this;
		}

		public ILicenseValidator<T> ExpiresAfter(
			DateTime dateTime,
			string message = "")
		{
			if (_licenseKey.ExpirationDate >= dateTime.ToUniversalTime())
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"expiration date {_licenseKey.ExpirationDate} after {dateTime}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfExpiresAfter(DateTime dateTime, Action<DateTime> action)
		{
			if (_licenseKey.ExpirationDate >= dateTime.ToUniversalTime())
				action(_licenseKey.ActivationDate);
			return this;
		}

		public ILicenseValidator<T> TypeIs(
			LicenseType licenseType,
			string message = "")
		{
			if (_licenseKey.LicenseType == licenseType)
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"license requires {Enum.GetName(typeof(LicenseType), licenseType)}, "
				+ $"got {Enum.GetName(typeof(LicenseType), _licenseKey.LicenseType)} is not valid"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfTypeIs(LicenseType licenseType, Action<LicenseType> action)
		{
			if (_licenseKey.LicenseType == licenseType)
				action(_licenseKey.LicenseType);
			return this;
		}

		public ILicenseValidator<T> TypeNot(
			LicenseType licenseType,
			string message = "")
		{
			if (_licenseKey.LicenseType != licenseType)
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"license cannot be {Enum.GetName(typeof(LicenseType), licenseType)}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfTypeNot(LicenseType licenseType, Action<LicenseType> action)
		{
			if (_licenseKey.LicenseType != licenseType)
				action(_licenseKey.LicenseType);
			return this;
		}

		public ILicenseValidator<T> NameIs(
			string licenseName,
			string message = "")
		{
			if (_licenseKey.LicenseName == licenseName)
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"license name \"{_licenseKey.LicenseName}\" does not match {licenseName}"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfNameIs(string licenseName, Action<string> action)
		{
			if (_licenseKey.LicenseName == licenseName)
				action(_licenseKey.LicenseName);
			return this;
		}

		public ILicenseValidator<T> NameNot(
			string licenseName,
			string message = "")
		{
			if (_licenseKey.LicenseName != licenseName)
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? $"license name should not be \"{_licenseKey.LicenseName}\""
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfNameNot(string licenseName, Action<string> action)
		{
			if (_licenseKey.LicenseName != licenseName)
				action(_licenseKey.LicenseName);
			return this;
		}

		public ILicenseValidator<T> MeetsCondition(
			Func<LicenseKey<T>, bool> condition,
			string message = "")
		{
			if (condition(_licenseKey))
				return this;

			message = string.IsNullOrWhiteSpace(message)
				? "check failed custom condition"
				: message;

			_errors.Add(message);
			return this;
		}

		public ILicenseValidator<T> IfConditionMet(
			Func<LicenseKey<T>, bool> condition,
			Action<LicenseKey<T>> action)
		{
			if (condition(_licenseKey))
				action(_licenseKey);
			return this;
		}

		public ILicenseValidator<T> GetLicenseKey(out LicenseKey <T>licenseKey)
		{
			licenseKey = _licenseKey;
			return this;
		}

		public LicenseValidationResults<T> Validate()
			=> new LicenseValidationResults<T>
			{
				Errors = _errors.AsReadOnly(), LicenseKey = _licenseKey
			};
	}
}