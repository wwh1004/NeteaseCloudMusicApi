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
	public sealed partial class CloudMusicApi {
		/// <summary>
		/// Cookies
		/// </summary>
		public CookieCollection Cookies { get; }

		/// <summary>
		/// 请求头中的 X-Real-IP，如果为 <see langword="null"/> 则不设置
		/// </summary>
		public string RealIP { get; set; }

		/// <summary>
		/// 是否使用代理
		/// </summary>
		public bool UseProxy { get; set; }

		/// <summary>
		/// 代理
		/// </summary>
		public IWebProxy Proxy { get; set; }

		/// <summary>
		/// 构造器
		/// </summary>
		public CloudMusicApi() {
			Cookies = new CookieCollection();
			UseProxy = true;
		}

		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="cookies"></param>
		public CloudMusicApi(CookieCollection cookies) {
			if (cookies is null)
				throw new ArgumentNullException(nameof(cookies));

			Cookies = new CookieCollection { cookies };
		}

		/// <summary>
		/// 构造器
		/// </summary>
		/// <param name="cookies"></param>
		public CloudMusicApi(IEnumerable<Cookie> cookies) {
			if (cookies is null)
				throw new ArgumentNullException(nameof(cookies));

			Cookies = new CookieCollection();
			foreach (var cookie in cookies)
				Cookies.Add(cookie);
		}

		/// <summary>
		/// API请求
		/// </summary>
		/// <param name="provider">API提供者</param>
		/// <param name="queries">参数</param>
		/// <param name="throwIfFailed">如果请求失败，抛出异常</param>
		/// <returns></returns>
		public async Task<JObject> RequestAsync(CloudMusicApiProvider provider, Dictionary<string, object> queries = null, bool throwIfFailed = true) {
			if (provider is null)
				throw new ArgumentNullException(nameof(provider));

			if (queries is null)
				queries = new Dictionary<string, object>();
			JObject json;
			if (provider == CloudMusicApiProviders.CheckMusic)
				json = await HandleCheckMusicAsync(queries);
			else if (provider == CloudMusicApiProviders.Login)
				json = await HandleLoginAsync(queries);
			else if (provider == CloudMusicApiProviders.LoginStatus)
				json = await HandleLoginStatusAsync();
			else if (provider == CloudMusicApiProviders.RelatedPlaylist)
				json = await HandleRelatedPlaylistAsync(queries);
			else
				json = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			if (throwIfFailed && !IsSuccess(json))
				throw new HttpRequestException($"调用 '{provider.Route}' 失败");
			return json;
		}

		/// <summary>
		/// API是否请求成功
		/// </summary>
		/// <param name="json">服务器返回的数据</param>
		/// <returns></returns>
		public static bool IsSuccess(JObject json) {
			if (json is null)
				throw new ArgumentNullException(nameof(json));

			int code = (int)json["code"];
			return 200 <= code && code <= 299;
		}

		private async Task<JObject> RequestAsync(HttpMethod method, string url, Dictionary<string, object> data, Options options) {
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			var json = await Request.CreateRequest(method.Method, url, data, MergeOptions(options), Cookies);
			if ((int)json["code"] == 301)
				json["msg"] = "未登录";
			return json;
		}

		private Options MergeOptions(Options options) {
			var newOptions = new Options {
				Crypto = options.Crypto,
				Cookie = new CookieCollection(),
				UA = options.UA,
				Url = options.Url,
				RealIP = RealIP,
				UseProxy = UseProxy,
				Proxy = Proxy
			};
			newOptions.Cookie.Add(options.Cookie);
			newOptions.Cookie.Add(Cookies);
			return newOptions;
		}

		private async Task<JObject> HandleCheckMusicAsync(Dictionary<string, object> queries) {
			var provider = CloudMusicApiProviders.CheckMusic;
			var json = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			bool playable = (int)json["code"] == 200 && (int)json["data"][0]["code"] == 200;
			var result = new JObject {
				["success"] = playable,
				["message"] = playable ? "ok" : "亲爱的,暂无版权"
			};
			return result;
		}

		private async Task<JObject> HandleLoginAsync(Dictionary<string, object> queries) {
			var provider = CloudMusicApiProviders.Login;
			var json = await RequestAsync(provider.Method, provider.Url(queries), provider.Data(queries), provider.Options);
			if ((int)json["code"] == 502) {
				json = new JObject {
					["code"] = 502,
					["message"] = "账号或密码错误"
				};
			}
			return json;
		}

		private async Task<JObject> HandleLoginStatusAsync() {
			try {
				const string GUSER = "GUser=";
				const string GBINDS = "GBinds=";

				byte[] data = await QuickHttp.SendAsync("https://music.163.com", "Get", $"Cookie: {QuickHttp.ToCookieHeader(Cookies)}");
				string s = Encoding.UTF8.GetString(data);
				int index = s.IndexOf(GUSER, StringComparison.Ordinal);
				if (index == -1)
					return new JObject { ["code"] = 301 };
				var json = new JObject { ["code"] = 200 };
				using (var reader = new StringReader(s.Substring(index + GUSER.Length)))
				using (var jsonReader = new JsonTextReader(reader))
					json.Add("profile", JObject.Load(jsonReader));
				index = s.IndexOf(GBINDS, StringComparison.Ordinal);
				if (index == -1)
					return new JObject { ["code"] = 301 };
				using (var reader = new StringReader(s.Substring(index + GBINDS.Length)))
				using (var jsonReader = new JsonTextReader(reader))
					json.Add("bindings", JArray.Load(jsonReader));
				return json;
			}
			catch {
				return new JObject { ["code"] = 301 };
			}
		}

		private async Task<JObject> HandleRelatedPlaylistAsync(Dictionary<string, object> queries) {
			try {
				byte[] data = await QuickHttp.SendAsync($"https://music.163.com/playlist?id={queries["id"]}", "Get", $"User-Agent: {Request.ChooseUserAgent("pc")}");
				string s = Encoding.UTF8.GetString(data);
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
				return new JObject {
					["code"] = 200,
					["playlists"] = playlists
				};
			}
			catch (Exception ex) {
				return new JObject {
					["code"] = 500,
					["msg"] = ex.ToFullString()
				};
			}
		}
	}
}
