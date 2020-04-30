using System;
using System.Text;

using Newtonsoft.Json;

namespace Standard.Licensing.LicenseKey.Internals.Extensions
{
	internal static class JsonExtensions
	{
		internal static string ToJsonString(this object value)
			=> JsonConvert.SerializeObject(value);

		internal static string ToBase64JsonString(this object value)
			=> Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToJsonString()));

		internal static T FromJson<T>(this string value)
			=> JsonConvert.DeserializeObject<T>(value);

		internal static T FromBase64JsonString<T>(this string value)
			=> Encoding.UTF8.GetString(Convert.FromBase64String(value))
			   .FromJson<T>();
	}
}
