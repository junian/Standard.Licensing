using System;
using System.Linq;

using Standard.Licensing.LicenseKey.TestLicenses;

using Xunit;

namespace Standard.Licensing.LicenseKey.KeyValidation
{
	public class StandardLicenseValidatorTests
	{
		private readonly DateTime _activationDate = DateTime.Today.AddDays(-7);
		private readonly DateTime _expirationDate = DateTime.Today.AddDays(7);
		private readonly DateTime _validThrough = DateTime.Today.AddDays(7);

		private const string LicenseName = "Test License";
		private const string IssuedTo = "Some User";

		private const LicenseType Type = LicenseType.Standard;

		private const int MaxUsers = 500;


		private readonly StandardLicenseValidator<SimpleTestLicense> _licenseValidator;

		public StandardLicenseValidatorTests()
		{
			var licenseKey = new LicenseKey<SimpleTestLicense>
			{
				LicenseType = Type,
				ExpirationDate = _expirationDate,
				ActivationDate = _activationDate,
				LicenseName = LicenseName,
				KeyData = new SimpleTestLicense
				{
					ValidThrough = _validThrough,
					IssuedTo = IssuedTo,
					MaxUsers = MaxUsers
				}
			};

			_licenseValidator = new StandardLicenseValidator<SimpleTestLicense>(licenseKey);
		}

		#region ActivatesBefore

		[Fact]
		public void TestActivatesBefore_GivenValidDate_Succeeds()
		{
			Assert.False(
				_licenseValidator.ActivatesBefore(DateTime.Today)
				   .Validate()
				   .HasErrors);
		}

		[Fact]
		public void TestActivatesBefore_GivenInvalidDate_Fails()
		{
			Assert.True(
				_licenseValidator.ActivatesBefore(DateTime.Today.AddDays(-14))
				   .Validate()
				   .HasErrors);
		}

		[Fact]
		public void TestActivatesBefore_GivenInvalidDateAndCustomError_Fails()
		{
			const string errorString = "failed";
			Assert.Equal(
				errorString,
				_licenseValidator.ActivatesBefore(DateTime.Today.AddDays(-14), errorString)
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);
		}

		[Fact]
		public void TestActivatesBefore_GivenSameDate_Succeeds()
		{
			Assert.False(
				_licenseValidator.ActivatesBefore(_activationDate)
				   .Validate()
				   .HasErrors);
		}

		#endregion

		#region IfActivatesBefore

		[Fact]
		public void TestIfActivatesBefore_GivenValidDate_Succeeds()
		{
			var called = false;
			_licenseValidator.IfActivatesBefore(DateTime.Today, date => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfActivatesBefore_GivenBadDate_Fails()
		{
			var called = false;
			_licenseValidator.IfActivatesBefore(DateTime.Today.AddDays(-14), date => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfActivatesBefore_GivenSameDate_Succeeds()
		{
			var called = false;
			_licenseValidator.IfActivatesBefore(_activationDate, date => called = true);
			Assert.True(called);
		}

		#endregion

		#region ActivatesAfter

		[Fact]
		public void TestActivatesAfter_GivenValidDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ActivatesAfter(DateTime.Today.AddDays(-14))
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestActivatesAfter_GivenInvalidDate_Fails()
			=> Assert.True(
				_licenseValidator.ActivatesAfter(DateTime.Today.AddDays(14))
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestActivatesAFter_GivenInvalidDateAndCustomError_Fails()
		{
			const string errorString = "error";
			Assert.Equal(
				errorString,
				_licenseValidator.ActivatesAfter(DateTime.Today.AddDays(14), errorString)
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);
		}

		[Fact]
		public void TestActivatesAfter_GivenSameDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ActivatesAfter(_activationDate)
				   .Validate()
				   .HasErrors);

		#endregion

		#region IfActivatesAfter

		[Fact]
		public void TestIfActivatesAfter_GivenValidDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfActivatesAfter(DateTime.Today, date => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfActivatesAfter_GivenInvalidDate_Fails()
		{
			bool called = false;
			_licenseValidator.IfActivatesAfter(DateTime.Today.AddDays(14), date => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfActivatesAfter_GivenSameDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfActivatesAfter(DateTime.Today, date => called = true);
			Assert.True(called);
		}

		#endregion

		#region ExpiresBefore

		[Fact]
		public void TestExpiresBefore_GivenValidDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ExpiresBefore(DateTime.Today.AddDays(14))
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestExpiresBefore_GivenBadDate_Fails()
			=> Assert.True(
				_licenseValidator.ExpiresBefore(DateTime.Today)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestExpiresBefore_GivenBadDateAndCustomError_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.ExpiresBefore(DateTime.Today, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		[Fact]
		public void TestExpiresBefore_GivenSameDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ExpiresBefore(_expirationDate)
				   .Validate()
				   .HasErrors);

		#endregion

		#region IfExpiresBefore

		[Fact]
		public void TestIfExpiresBefore_GivenValidDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfExpiresBefore(DateTime.Today.AddDays(14), time => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfExpiresBefore_GivenBadDate_Fails()
		{
			bool called = false;
			_licenseValidator.IfExpiresBefore(DateTime.Today, time => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfExpiresBefore_GivenSameDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfExpiresBefore(_expirationDate, time => called = true);
			Assert.True(called);
		}

		#endregion

		#region ExpiresAfter

		[Fact]
		public void TestExpiresAfter_GivenValidDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ExpiresAfter(DateTime.Today)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestExpiresAfter_GivenBadDate_Fails()
			=> Assert.True(
				_licenseValidator.ExpiresAfter(DateTime.Today.AddDays(14))
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestExpiresAfter_GivenBadDateAndCustomError_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.ExpiresAfter(DateTime.Today.AddDays(14), "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		[Fact]
		public void TestExpiresAfter_GivenSameDate_Succeeds()
			=> Assert.False(
				_licenseValidator.ExpiresAfter(_expirationDate)
				   .Validate()
				   .HasErrors);

		#endregion

		#region IfExpiresAfter

		[Fact]
		public void TestIfExpiresAfter_GivenValidDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfExpiresAfter(DateTime.Today, _ => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfExpiresAfter_GivenBadDate_Fails()
		{
			bool called = false;
			_licenseValidator.IfExpiresAfter(DateTime.Today.AddDays(14), _ => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfExpiresAfter_GivenSameDate_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfExpiresAfter(_expirationDate, _ => called = true);
			Assert.True(called);
		}

		#endregion

		#region TypeIs

		[Fact]
		public void TestTypeIs_GivenValidType_Succeeds()
			=> Assert.False(
				_licenseValidator.TypeIs(Type)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestTypeIs_GivenWrongType_Fails()
			=> Assert.True(
				_licenseValidator.TypeIs(LicenseType.Enterprise)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestTypeIs_GivenWrongTypeAndErrorMessage_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.TypeIs(LicenseType.Enterprise, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		#endregion

		#region IfTypeIs

		[Fact]
		public void TestIfTypeIs_GivenValidType_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfTypeIs(Type, _ => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfTypeIs_GivenBadType_Fails()
		{
			bool called = false;
			_licenseValidator.IfTypeIs(LicenseType.Enterprise, _ => called = true);
			Assert.False(called);
		}

		#endregion

		#region TypeNot

		[Fact]
		public void TestTypeNot_GivenValidType_Fails()
			=> Assert.True(
				_licenseValidator.TypeNot(Type)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestTypeNot_GivenValidTypeAndErrorMessage_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.TypeNot(Type, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		[Fact]
		public void TestTypeNot_GivenBadType_Succeeds()
			=> Assert.False(
				_licenseValidator.TypeNot(LicenseType.Enterprise)
				   .Validate()
				   .HasErrors);

		#endregion

		#region IfTypeNot

		[Fact]
		public void TestIfTypeNot_GivenValidType_Fails()
		{
			bool called = false;
			_licenseValidator.IfTypeNot(Type, _ => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfTypeNot_GivenBadType_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfTypeNot(LicenseType.Enterprise, _ => called = true);
			Assert.True(called);
		}

		#endregion

		#region NameIs

		[Fact]
		public void TestNameIs_GivenValidName_Succeeds()
			=> Assert.False(
				_licenseValidator.NameIs(LicenseName)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestNameIs_GivenInvalidName_Fails()
			=> Assert.True(
				_licenseValidator.NameIs(string.Empty)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestNameIs_GivenInvalidNameAndErrorMessage_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.NameIs(string.Empty, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		#endregion

		#region IfNameIs

		[Fact]
		public void TestIfNameIs_GivenValidName_Succeeds()
		{
			bool called = true;
			_licenseValidator.IfNameIs(LicenseName, _ => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfNameIs_GivenInvalidName_Fails()
		{
			bool called = true;
			_licenseValidator.IfNameIs(string.Empty, _ => called = true);
			Assert.True(called);
		}

		#endregion

		#region NameNot

		[Fact]
		public void TestNameNot_GivenValidName_Fails()
			=> Assert.True(
				_licenseValidator.NameNot(LicenseName)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestNameNot_GivenValidNameAndErrorMessage_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.NameNot(LicenseName, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		[Fact]
		public void TestNameNot_GivenInvalidName_Succeeds()
			=> Assert.False(
				_licenseValidator.NameNot(string.Empty)
				   .Validate()
				   .HasErrors);

		#endregion

		#region IfNameNot

		[Fact]
		public void TestIfNameNot_GivenValidName_Fails()
		{
			bool called = false;
			_licenseValidator.IfNameNot(LicenseName, _ => called = true);
			Assert.False(called);
		}

		[Fact]
		public void TestIfNameNot_GivenInvalidName_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfNameNot(string.Empty, _ => called = true);
			Assert.True(called);
		}

		#endregion

		#region MeetsCondition

		[Fact]
		public void TestMeetsCondition_GivenValidCondition_Succeeds()
			=> Assert.False(
				_licenseValidator.MeetsCondition(lic => lic.LicenseName == LicenseName)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestMeetsCondition_GivenInvalidCondition_Fails()
			=> Assert.True(
				_licenseValidator.MeetsCondition(lic => lic.LicenseName != LicenseName)
				   .Validate()
				   .HasErrors);

		[Fact]
		public void TestMeetsCondition_GivenInvalidConditionWithMessage_Fails()
			=> Assert.Equal(
				"error",
				_licenseValidator.MeetsCondition(lic => lic.LicenseName != LicenseName, "error")
				   .Validate()
				   .Errors.FirstOrDefault()
				?? string.Empty);

		#endregion

		#region IfConditionMet

		[Fact]
		public void TestIfConditionMet_GivenValidCondition_Succeeds()
		{
			bool called = false;
			_licenseValidator.IfConditionMet(
				lic => lic.LicenseName == LicenseName,
				_ => called = true);
			Assert.True(called);
		}

		[Fact]
		public void TestIfConditonMet_GivenInvalidConditon_Fails()
		{
			bool called = false;
			_licenseValidator.IfConditionMet(
				lic => lic.LicenseName != LicenseName,
				_ => called = true);
			Assert.False(called);
		}

		#endregion
	}
}