using System;

namespace Standard.Licensing.LicenseKey.TestLicenses
{
	public class SimpleTestLicense
	{
		public int MaxUsers { get; set; }

		public string IssuedTo { get; set; } = null!;

		public DateTime ValidThrough { get; set; }

		public static SimpleTestLicense CreateDefaultTestLicense()
			=> new SimpleTestLicense
			{
				MaxUsers = 100,
				IssuedTo = "Me",
				ValidThrough = DateTime.Today.AddDays(7)
			};

		public override bool Equals(object? obj)
			=> obj != null
				&& obj is SimpleTestLicense testLicense
				&& MaxUsers == testLicense.MaxUsers
				&& IssuedTo == testLicense.IssuedTo
				&& ValidThrough == testLicense.ValidThrough;

		protected bool Equals(SimpleTestLicense other)
			=> MaxUsers == other.MaxUsers && IssuedTo == other.IssuedTo && ValidThrough.Equals(other.ValidThrough);

		public override int GetHashCode()
			=> HashCode.Combine(MaxUsers, IssuedTo, ValidThrough);
	}
}