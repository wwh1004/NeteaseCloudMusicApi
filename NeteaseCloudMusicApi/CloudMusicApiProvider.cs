using System;
using System.Collections.Generic;
using System.Net.Http;
using NeteaseCloudMusicApi.Utils;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 网易云音乐API提供者
	/// </summary>
	public sealed class CloudMusicApiProvider {
		/// <summary />
		public string Route { get; }

		internal HttpMethod Method { get; }

		internal Func<Dictionary<string, string>, string> Url { get; }

		internal ParameterInfo[] ParameterInfos { get; }

		internal Options Options { get; }

		internal Func<Dictionary<string, string>, Dictionary<string, string>> DataProvider { get; set; }

		internal Func<Dictionary<string, string>, Dictionary<string, string>> Data => DataProvider ?? GetData;

		internal CloudMusicApiProvider(string router) {
			if (string.IsNullOrEmpty(router))
				throw new ArgumentNullException(nameof(router));

			Route = router;
		}

		internal CloudMusicApiProvider(string router, HttpMethod method, Func<Dictionary<string, string>, string> url, ParameterInfo[] parameterInfos, Options options) {
			if (string.IsNullOrEmpty(router))
				throw new ArgumentNullException(nameof(router));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (parameterInfos is null)
				throw new ArgumentNullException(nameof(parameterInfos));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			Route = router;
			Method = method;
			Url = url;
			ParameterInfos = parameterInfos;
			Options = options;
		}

		private Dictionary<string, string> GetData(Dictionary<string, string> queries) {
			if (ParameterInfos.Length == 0)
				return new Dictionary<string, string>();
			var data = new Dictionary<string, string>();
			foreach (var parameterInfo in ParameterInfos) {
				switch (parameterInfo.Type) {
				case ParameterType.Required:
					data.Add(parameterInfo.Key, parameterInfo.GetRealValue(queries[parameterInfo.GetForwardedKey()]));
					break;
				case ParameterType.Optional:
					data.Add(parameterInfo.Key, queries.TryGetValue(parameterInfo.GetForwardedKey(), out string value) ? parameterInfo.GetRealValue(value) : parameterInfo.DefaultValue);
					break;
				case ParameterType.Constant:
					data.Add(parameterInfo.Key, parameterInfo.DefaultValue);
					break;
				case ParameterType.Custom:
					data.Add(parameterInfo.Key, parameterInfo.CustomHandler(queries));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(parameterInfo));
				}
			}
			return data;
		}

		/// <summary />
		public override string ToString() {
			return Route;
		}

		internal enum ParameterType {
			Required,
			Optional,
			Constant,
			Custom
		}

		internal sealed class ParameterInfo {
			public string Key;
			public ParameterType Type;
			public string DefaultValue;
			public string KeyForwarding;
			public Func<string, string> Transformer;
			public Func<Dictionary<string, string>, string> CustomHandler;

			public ParameterInfo(string key) : this(key, ParameterType.Required, null) {
			}

			public ParameterInfo(string key, ParameterType type, string defaultValue) {
				Key = key;
				Type = type;
				DefaultValue = defaultValue;
			}

			public string GetForwardedKey() {
				return KeyForwarding ?? Key;
			}

			public string GetRealValue(string value) {
				return Transformer is null ? value : Transformer(value);
			}
		}
	}
}
