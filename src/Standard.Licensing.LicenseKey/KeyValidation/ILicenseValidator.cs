using System;

namespace Standard.Licensing.LicenseKey.KeyValidation
{
	public interface ILicenseValidator<T>
	{

		/// <summary>
		/// checks to see if the license activates before the given date
		/// </summary>
		/// <param name="dateTime">the date / time that the license should activate before</param>
		/// <param name="message">a message to be logged in errors/warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> ActivatesBefore(
			DateTime dateTime,
			string message = "");

		/// <summary>
		/// if the license activates before the given date, run the specified action
		/// </summary>
		/// <param name="dateTime">the date / time that the license should activate before to trigger the action</param>
		/// <param name="action">the action that should run if the condition is met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfActivatesBefore(DateTime dateTime, Action<DateTime> action);

		/// <summary>
		/// check to see if the license activates after the given date
		/// </summary>
		/// <param name="dateTime">the date / time that the license should activate after</param>
		/// <param name="message">a message to be logged in errors/warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> ActivatesAfter(
			DateTime dateTime,
			string message = "");

		/// <summary>
		/// checks to see if the license activates after the given date, if true runs an action
		/// </summary>
		/// <param name="dateTime">the date / time that the license should activate after to trigger the action</param>
		/// <param name="action">the action that should be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfActivatesAfter(DateTime dateTime, Action<DateTime> action);

		/// <summary>
		/// check to see if the license expires before the given date / time
		/// </summary>
		/// <param name="dateTime">the date / time that the license should expire before</param>
		/// <param name="message">the message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> ExpiresBefore(
			DateTime dateTime,
			string message = "");

		/// <summary>
		/// checks to see if the license expires before the given date, if true runs an action
		/// </summary>
		/// <param name="dateTime">the date / time that the license should expire before to trigger the action</param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfExpiresBefore(DateTime dateTime, Action<DateTime> action);

		/// <summary>
		/// check to see if the license expires after the given date / time
		/// </summary>
		/// <param name="dateTime">the date / time that the license should expire after</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> ExpiresAfter(
			DateTime dateTime,
			string message = "");

		/// <summary>
		/// checks if the license expires after the given date / time, if true runs an action
		/// </summary>
		/// <param name="dateTime">the date / time that the license should expire after to trigger the action</param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfExpiresAfter(DateTime dateTime, Action<DateTime> action);

		/// <summary>
		/// check if the license type is a certain type
		/// </summary>
		/// <param name="licenseType">the type of license that the license should be</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> TypeIs(
			LicenseType licenseType,
			string message = "");

		/// <summary>
		/// checks if the license is of a certain type, if true runs an action
		/// </summary>
		/// <param name="licenseType">the type of license that the license should be to trigger the action</param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfTypeIs(LicenseType licenseType, Action<LicenseType> action);

		/// <summary>
		/// check to see if the license type is not a certain value
		/// </summary>
		/// <param name="licenseType">the type of license that our license should not be</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> TypeNot(
			LicenseType licenseType,
			string message = "");

		/// <summary>
		/// checks if the license is not of a certain type, if true runs an action
		/// </summary>
		/// <param name="licenseType">the type of license that the license should not be to trigger the action </param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfTypeNot(LicenseType licenseType, Action<LicenseType> action);

		/// <summary>
		/// checks if the license has a certain name
		/// </summary>
		/// <param name="licenseName">the name that the license should have</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> NameIs(
			string licenseName,
			string message = "");

		/// <summary>
		/// checks to see if a license name is a certain value, if true runs an action
		/// </summary>
		/// <param name="licenseName">the name that the license should have</param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfNameIs(string licenseName, Action<string> action);

		/// <summary>
		/// checks to see if a license name is not a certain value, if that value logs an error
		/// </summary>
		/// <param name="licenseName">the name that the license should not have</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> NameNot(
			string licenseName,
			string message = "");

		/// <summary>
		/// checks to see if the name is not a certain value, if true runs an action
		/// </summary>
		/// <param name="licenseName">the name that the license should not have</param>
		/// <param name="action">the action that will be run on condition met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfNameNot(string licenseName, Action<string> action);

		/// <summary>
		/// checks to see if the license meets a certain user-specified condition
		/// </summary>
		/// <param name="condition">the condition that should be true to validate the license</param>
		/// <param name="message">a message to be logged in errors / warnings if the validation fails</param>
		/// <returns></returns>
		ILicenseValidator<T> MeetsCondition(
			Func<LicenseKey<T>, bool> condition,
			string message = "");

		/// <summary>
		/// checks if a custom user-defined condition is met, if true runs an action
		/// </summary>
		/// <param name="condition">a custom user-defined condition that should be met to run the action</param>
		/// <param name="action">the action that will be run if the condition is met</param>
		/// <returns></returns>
		ILicenseValidator<T> IfConditionMet(
			Func<LicenseKey<T>, bool> condition,
			Action<LicenseKey<T>> action);

		/// <summary>
		/// get the underlying license from the validator
		/// </summary>
		/// <param name="licenseKey">the underlying license key</param>
		/// <returns></returns>
		ILicenseValidator<T> GetLicenseKey(out LicenseKey<T> licenseKey);

		LicenseValidationResults<T> Validate();
	}
}