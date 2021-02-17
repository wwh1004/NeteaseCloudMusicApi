using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NeteaseCloudMusicApi.util {
	internal static class Extensions {
		private static readonly MD5 _md5 = MD5.Create();

		public static byte[] ToByteArrayUtf8(this string value) {
			return Encoding.UTF8.GetBytes(value);
		}

		public static string ToHexStringLower(this byte[] value) {
			var sb = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
				sb.Append(value[i].ToString("x2"));
			return sb.ToString();
		}

		public static string ToHexStringUpper(this byte[] value) {
			var sb = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
				sb.Append(value[i].ToString("X2"));
			return sb.ToString();
		}

		public static string ToBase64String(this byte[] value) {
			return Convert.ToBase64String(value);
		}

		public static byte[] ComputeMd5(this byte[] value) {
			return _md5.ComputeHash(value);
		}

		public static byte[] RandomBytes(this Random random, int length) {
			byte[] buffer = new byte[length];
			random.NextBytes(buffer);
			return buffer;
		}

		public static string Get(this CookieCollection cookies, string name, string defaultValue) {
			return cookies[name]?.Value ?? defaultValue;
		}
	}
}
