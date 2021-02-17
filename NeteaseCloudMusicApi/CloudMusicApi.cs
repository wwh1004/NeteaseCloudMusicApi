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
using NeteaseCloudMusicApi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 网易云音乐API
	/// </summary>
	public sealed partial class CloudMusicApi : IDisposable {
		private bool _isDisposed;

		/// <summary />
		public HttpClient Client { get; }

		/// <summary />
		public HttpClientHandler ClientHandler { get; }

		/// <summary>
		/// 空请求参数，用于填充 queries 参数
		/// </summary>
		public static Dictionary<string, string> EmptyQueries { get; } = new Dictionary<string, string>();

		/// <summary>
		/// 构造器
		/// </summary>
		public CloudMusicApi() {
			ClientHandler = new HttpClientHandler {
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				UseCookies = true
			};
			Client = new HttpClient(ClientHandler);
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
			else if (provider == CloudMusicApiProviders.Login)
				return HandleLoginAsync(queries);
			else if (provider == CloudMusicApiProviders.LoginStatus)
				return HandleLoginStatusAsync();
			else if (provider == CloudMusicApiProviders.RelatedPlaylist)
				return HandleRelatedPlaylistAsync(queries);
			return RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
		}

		private async Task<(bool, JObject)> RequestAsync(HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> data, Options options) {
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			var data2 = new Dictionary<string, string>();
			foreach (var item in data)
				data2.Add(item.Key, item.Value);
			var (isOk, json) = await Utils.Request.CreateRequest(Client, method.Method, url, data2, options);
			json = (JObject)json["body"];
			if (!isOk && (int?)json["code"] == 301)
				json["msg"] = "需要登录";
			return (isOk, json);
		}

		private async Task<(bool, JObject)> HandleCheckMusicAsync(Dictionary<string, string> queries) {
			var provider = CloudMusicApiProviders.CheckMusic;
			var (isOk, json) = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			if (!isOk)
				return (false, null);
			bool playable = (int?)json["code"] == 200 && (int?)json.SelectToken("data[0].code") == 200;
			var result = new JObject {
				["success"] = playable,
				["message"] = playable ? "ok" : "亲爱的,暂无版权"
			};
			return (true, result);
		}

		private async Task<(bool, JObject)> HandleLoginAsync(Dictionary<string, string> queries) {
			var provider = CloudMusicApiProviders.Login;
			var (isOk, json) = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			if (!isOk)
				return (false, null);
			if ((int?)json["code"] == 502) {
				json = new JObject {
					["msg"] = "账号或密码错误",
					["code"] = 502,
					["message"] = "账号或密码错误"
				};
			}
			return (isOk, json);
		}

		private async Task<(bool, JObject)> HandleLoginStatusAsync() {
			try {
				const string GUSER = "GUser=";
				const string GBINDS = "GBinds=";

				using var response = await Client.GetAsync("https://music.163.com");
				string s = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
				int index = s.IndexOf(GUSER, StringComparison.Ordinal);
				if (index == -1)
					return (false, new JObject { ["code"] = 301 });
				var json = new JObject { ["code"] = 200 };
				using (var reader = new StringReader(s.Substring(index + GUSER.Length)))
				using (var jsonReader = new JsonTextReader(reader))
					json.Add("profile", JObject.Load(jsonReader));
				index = s.IndexOf(GBINDS, StringComparison.Ordinal);
				if (index == -1)
					return (false, new JObject { ["code"] = 301 });
				using (var reader = new StringReader(s.Substring(index + GBINDS.Length)))
				using (var jsonReader = new JsonTextReader(reader))
					json.Add("bindings", JArray.Load(jsonReader));
				return (true, json);
			}
			catch {
				return (false, new JObject { ["code"] = 301 });
			}
		}

		private async Task<(bool, JObject)> HandleRelatedPlaylistAsync(Dictionary<string, string> queries) {
			try {
				using var response = await Client.SendAsync("https://music.163.com/playlist", HttpMethod.Get, new Dictionary<string, string> { ["id"] = queries["id"] }, new Dictionary<string, string> { ["User-Agent"] = Utils.Request.ChooseUserAgent("pc") });
				string s = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
				var matchs = Regex.Matches(s, @"<div class=""cver u-cover u-cover-3"">[\s\S]*?<img src=""([^""]+)"">[\s\S]*?<a class=""sname f-fs1 s-fc0"" href=""([^""]+)""[^>]*>([^<]+?)<\/a>[\s\S]*?<a class=""nm nm f-thide s-fc3"" href=""([^""]+)""[^>]*>([^<]+?)<\/a>");
				var playlists = new JArray(matchs.Cast<Match>().Select(match => new JObject {
					["creator"] = new JObject {
						["userId"] = match.Groups[4].Value.Substring("/user/home?id=".Length),
						["nickname"] = match.Groups[5].Value
					},
					["coverImgUrl"] = match.Groups[1].Value.Substring(0, match.Groups[1].Value.Length - "?param=50y50".Length),
					["name"] = match.Groups[3].Value,
					["id"] = match.Groups[2].Value.Substring("/playlist?id=".Length),
				}));
				return (true, new JObject {
					["code"] = 200,
					["playlists"] = playlists
				});
			}
			catch (Exception ex) {
				return (false, new JObject {
					["code"] = 500,
					["msg"] = ex.ToFullString()
				});
			}
		}

		/// <summary />
		public void Dispose() {
			if (!_isDisposed) {
				Client.Dispose();
				_isDisposed = true;
			}
		}
	}
}
