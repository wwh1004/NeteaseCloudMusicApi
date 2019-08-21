using System;
using System.Collections.Generic;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

			if (provider == CloudMusicApiProviders.CheckMusic)
				return HandleCheckMusicAsync(queries);
			else if (provider == CloudMusicApiProviders.LoginStatus)
				return HandleLoginStatusAsync();
			else if (provider == CloudMusicApiProviders.RelatedPlaylist)
				return HandleRelatedPlaylistAsync(queries);
			return RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
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
			if (!isOk && (int?)json["code"] == 301)
				json["msg"] = "需要登录";
			return (isOk, json);
		}

		private async Task<(bool, JObject)> HandleCheckMusicAsync(Dictionary<string, string> queries) {
			CloudMusicApiProvider provider;
			bool isOk;
			JObject json;
			JObject result;
			bool playable;

			provider = CloudMusicApiProviders.CheckMusic;
			(isOk, json) = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			if (!isOk)
				return (false, null);
			playable = (int?)json["code"] == 200 && (int?)json.SelectToken("data[0].code") == 200;
			result = new JObject {
				{ "success", playable },
				{ "message", playable ? "ok" : "亲爱的,暂无版权"}
			};
			return (true, result);
		}

		private async Task<(bool, JObject)> HandleLoginStatusAsync() {
			HttpResponseMessage response;

			response = null;
			try {
				const string GUSER = "GUser=";
				const string GBINDS = "GBinds=";

				string s;
				int index;
				JObject json;

				response = await _client.GetAsync("https://music.163.com");
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
			finally {
				response?.Dispose();
			}
		errorExit:
			return (false, new JObject {
				{ "code", 301 }
			});
		}

		private async Task<(bool, JObject)> HandleRelatedPlaylistAsync(Dictionary<string, string> queries) {
			HttpResponseMessage response;

			response = null;
			try {
				string s;
				MatchCollection matchs;
				JArray playlists;

				response = await _client.SendAsync(HttpMethod.Get, "https://music.163.com/playlist", new QueryCollection { { "id", queries["id"] } }, new QueryCollection { { "User-Agent", request.chooseUserAgent("pc") } });
				s = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
				matchs = Regex.Matches(s, @"<div class=""cver u-cover u-cover-3"">[\s\S]*?<img src=""([^""]+)"">[\s\S]*?<a class=""sname f-fs1 s-fc0"" href=""([^""]+)""[^>]*>([^<]+?)<\/a>[\s\S]*?<a class=""nm nm f-thide s-fc3"" href=""([^""]+)""[^>]*>([^<]+?)<\/a>");
				playlists = new JArray(matchs.Cast<Match>().Select(match => new JObject {
					{ "creator", new JObject {
						{ "userId", match.Groups[4].Value.Substring("/user/home?id=".Length) },
						{ "nickname", match.Groups[5].Value }
					} },
					{ "coverImgUrl", match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - "?param=50y50".Length) },
					{ "name", match.Groups[3].Value },
					{ "id", match.Groups[2].Value.Substring("/playlist?id=".Length) },
				}));
				return (true, new JObject {
					{ "code", 200 },
					{ "playlists", playlists }
				});
			}
			catch (Exception ex) {
				return (false, new JObject {
					{ "code", 500 },
					{ "msg", ex.ToFullString() }
				});
			}
			finally {
				response?.Dispose();
			}
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
