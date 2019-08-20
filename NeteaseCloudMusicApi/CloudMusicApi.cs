using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NeteaseCloudMusicApi.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 网易云音乐API
	/// </summary>
	public sealed partial class CloudMusicApi : IDisposable {
		private static readonly Dictionary<string, string> _emptyQueries = new Dictionary<string, string>();

		private readonly HttpClient _client;
		private readonly HttpClientHandler _clientHandler;
		private bool _isDisposed;

		/// <summary />
		public HttpClient Client => _client;

		/// <summary />
		public HttpClientHandler ClientHandler => _clientHandler;

		/// <summary>
		/// 代理服务器
		/// </summary>
		public IWebProxy Proxy {
			get => _clientHandler.Proxy;
			set => _clientHandler.Proxy = value;
		}

		/// <summary>
		/// 空请求参数，用于填充 queries 参数
		/// </summary>
		public static Dictionary<string, string> EmptyQueries => _emptyQueries;

		/// <summary>
		/// 构造器
		/// </summary>
		public CloudMusicApi() {
			_clientHandler = new HttpClientHandler {
				AutomaticDecompression = DecompressionMethods.None,
				UseCookies = true
			};
			_client = new HttpClient(_clientHandler);
		}

		/// <summary>
		/// API请求（如果.NET版本支持，请使用值元组异步版本 <see cref="RequestAsync(CloudMusicApiProvider, Dictionary{string, string})"/>）
		/// </summary>
		/// <param name="provider">API提供者</param>
		/// <param name="queries">参数</param>
		/// <param name="result">请求结果</param>
		/// <returns></returns>
		public bool Request(CloudMusicApiProvider provider, Dictionary<string, string> queries, out JObject result) {
			bool isOk;

			(isOk, result) = RequestAsync(provider, queries).Result;
			return isOk;
		}

		/// <summary>
		/// API请求
		/// </summary>
		/// <param name="provider">API提供者</param>
		/// <param name="queries">参数</param>
		/// <returns></returns>
		public Task<(bool, JObject)> RequestAsync(CloudMusicApiProvider provider, Dictionary<string, string> queries) {
			if (provider is null)
				throw new ArgumentNullException(nameof(provider));
			if (queries is null)
				throw new ArgumentNullException(nameof(queries));

			if (provider == CloudMusicApiProviders.LoginStatus)
				return HandleLoginStatus();
			return RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
		}

		private async Task<(bool, JObject)> HandleLoginStatus() {
			try {
				const string GUSER = "GUser=";
				const string GBINDS = "GBinds=";

				string s;
				int index;
				JObject json;

				using (HttpResponseMessage response = await _client.GetAsync("https://music.163.com"))
					s = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
				index = s.IndexOf(GUSER, StringComparison.Ordinal);
				if (index == -1)
					goto errorExit;
				json = new JObject {
					{ "code", 200 }
				};
				using (StringReader reader = new StringReader(s.Substring(index + GUSER.Length)))
				using (JsonReader jsonReader = new JsonTextReader(reader))
					json.Add("profile", JObject.Load(jsonReader));
				index = s.IndexOf(GBINDS, StringComparison.Ordinal);
				if (index == -1)
					goto errorExit;
				using (StringReader reader = new StringReader(s.Substring(index + GBINDS.Length)))
				using (JsonReader jsonReader = new JsonTextReader(reader))
					json.Add("bindings", JArray.Load(jsonReader));
				return (true, json);
			}
			catch {
				goto errorExit;
			}
		errorExit:
			return (false, new JObject {
				{ "code", 301 }
			});
		}

		private async Task<(bool, JObject)> RequestAsync(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> data, options options) {
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			bool isOk;
			JObject json;

			(isOk, json) = await request.createRequest(_client, method, url, data, options);
			json = (JObject)json["body"];
			if (!isOk && (int?)(json["code"] as JValue) == 301)
				json["msg"] = "需要登录";
			return (isOk, json);
		}


		/// <summary />
		public void Dispose() {
			if (_isDisposed)
				return;
			_clientHandler.Dispose();
			_client.Dispose();
			_isDisposed = true;
		}
	}
}
