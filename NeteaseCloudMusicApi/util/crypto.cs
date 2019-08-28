using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace NeteaseCloudMusicApi.util {
	internal static class crypto {
		private static readonly byte[] iv = Encoding.ASCII.GetBytes("0102030405060708");
		private static readonly byte[] presetKey = Encoding.ASCII.GetBytes("0CoJUm6Qyw8W8jud");
		private static readonly byte[] linuxapiKey = Encoding.ASCII.GetBytes("rFgB&h#%2?^eDg:Q");
		private const string base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		private const string publicKey = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDgtQn2JZ34ZC28NWYpAUd98iZ37BUrX/aKzmFbt7clFSs6sXqHauqKWqdtLkF2KexO40H1YTX8z2lSgBBOAxLsvaklV8k4cBFK9snQXE9/DDaFt6Rr7iVZMldczhC0JNgTz+SHXT6CBHuX3e9SdB1Ua44oncaTWz7OBGLbCiK45wIDAQAB\n-----END PUBLIC KEY-----";
		private static readonly byte[] eapiKey = Encoding.ASCII.GetBytes("e82ckenh8dichen8");

		private static RSAParameters? _cachedPublicKey;

		public static Dictionary<string, string> weapi(object @object) {
			string text;
			byte[] secretKey;

			text = JsonConvert.SerializeObject(@object);
			secretKey = new Random().RandomBytes(16);
			secretKey = secretKey.Select(n => (byte)base62[n % 62]).ToArray();
			return new Dictionary<string, string> {
				{ "params", aesEncrypt(aesEncrypt(text.ToByteArrayUtf8(), CipherMode.CBC, presetKey, iv).ToBase64String().ToByteArrayUtf8(), CipherMode.CBC, secretKey, iv).ToBase64String() },
				{ "encSecKey", rsaEncrypt(secretKey.Reverse().ToArray()/*, publicKey*/).ToHexStringLower() }
			};
		}

		public static Dictionary<string, string> linuxapi(object @object) {
			string text;

			text = JsonConvert.SerializeObject(@object);
			return new Dictionary<string, string> {
				{ "eparams", aesEncrypt(text.ToByteArrayUtf8(), CipherMode.ECB, linuxapiKey, null).ToHexStringUpper() }
			};
		}

		public static Dictionary<string, string> eapi(string url, object @object) {
			string text;
			string message;
			string digest;
			string data;

			text = JsonConvert.SerializeObject(@object);
			message = $"nobody{url}use{text}md5forencrypt";
			digest = message.ToByteArrayUtf8().ComputeMd5().ToHexStringLower();
			data = $"{url}-36cd479b6b5-{text}-36cd479b6b5-{digest}";
			return new Dictionary<string, string> {
				{ "params", aesEncrypt(data.ToByteArrayUtf8(), CipherMode.ECB, eapiKey, null).ToHexStringUpper() }
			};
		}

		public static byte[] decrypt(byte[] cipherBuffer) {
			return aesDecrypt(cipherBuffer, CipherMode.ECB, eapiKey, null);
		}

		private static byte[] aesEncrypt(byte[] buffer, CipherMode mode, byte[] key, byte[] iv) {
			using (Aes aes = Aes.Create()) {
				aes.BlockSize = 128;
				aes.Key = key;
				if (!(iv is null))
					aes.IV = iv;
				aes.Mode = mode;
				using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
					return cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
			}
		}

		private static byte[] aesDecrypt(byte[] buffer, CipherMode mode, byte[] key, byte[] iv) {
			using (Aes aes = Aes.Create()) {
				aes.BlockSize = 128;
				aes.Key = key;
				if (!(iv is null))
					aes.IV = iv;
				aes.Mode = mode;
				using (ICryptoTransform cryptoTransform = aes.CreateDecryptor())
					return cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
			}
		}

		private static byte[] rsaEncrypt(byte[] buffer/*, string key*/) {
			RSAParameters rsaParameters;

			if (_cachedPublicKey is null)
				_cachedPublicKey = ParsePublicKey(publicKey);
			rsaParameters = _cachedPublicKey.Value;
			return BigInteger.ModPow(GetBigIntegerBigEndian(buffer), GetBigIntegerBigEndian(rsaParameters.Exponent), GetBigIntegerBigEndian(rsaParameters.Modulus)).ToByteArray(true, true);

			RSAParameters ParsePublicKey(string _publicKey) {
				_publicKey = _publicKey.Replace("\n", string.Empty);
				_publicKey = _publicKey.Substring(26, _publicKey.Length - 50);
				using (MemoryStream _stream = new MemoryStream(Convert.FromBase64String(_publicKey))) {
					using (BinaryReader _reader = new BinaryReader(_stream)) {
						ushort _i16;
						byte[] _oid;
						byte _i8;
						byte _low;
						byte _high;
						int _modulusLength;
						byte[] _modulus;
						int _exponentLength;
						byte[] _exponent;

						_i16 = _reader.ReadUInt16();
						if (_i16 == 0x8130)
							_reader.ReadByte();
						else if (_i16 == 0x8230)
							_reader.ReadInt16();
						else
							throw new ArgumentException(nameof(_publicKey));
						_oid = _reader.ReadBytes(15);
						if (!_oid.SequenceEqual(new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 }))
							throw new ArgumentException(nameof(_publicKey));
						_i16 = _reader.ReadUInt16();
						if (_i16 == 0x8103)
							_reader.ReadByte();
						else if (_i16 == 0x8203)
							_reader.ReadInt16();
						else
							throw new ArgumentException(nameof(_publicKey));
						_i8 = _reader.ReadByte();
						if (_i8 != 0x00)
							throw new ArgumentException(nameof(_publicKey));
						_i16 = _reader.ReadUInt16();
						if (_i16 == 0x8130)
							_reader.ReadByte();
						else if (_i16 == 0x8230)
							_reader.ReadInt16();
						else
							throw new ArgumentException(nameof(_publicKey));
						_i16 = _reader.ReadUInt16();
						if (_i16 == 0x8102) {
							_high = 0;
							_low = _reader.ReadByte();
						}
						else if (_i16 == 0x8202) {
							_high = _reader.ReadByte();
							_low = _reader.ReadByte();
						}
						else
							throw new ArgumentException(nameof(_publicKey));
						_modulusLength = BitConverter.ToInt32(new byte[] { _low, _high, 0x00, 0x00 }, 0);
						if (_reader.PeekChar() == 0x00) {
							_reader.ReadByte();
							_modulusLength -= 1;
						}
						_modulus = _reader.ReadBytes(_modulusLength);
						if (_reader.ReadByte() != 0x02)
							throw new ArgumentException(nameof(_publicKey));
						_exponentLength = _reader.ReadByte();
						_exponent = _reader.ReadBytes(_exponentLength);
						return new RSAParameters {
							Modulus = _modulus,
							Exponent = _exponent
						};
					}
				}
			}

			BigInteger GetBigIntegerBigEndian(byte[] _value) {
				return new BigInteger(new ReadOnlySpan<byte>(_value), true, true);
			}
		}
	}
}
