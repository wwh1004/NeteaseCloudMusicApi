using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusicApi.Utils {
	internal static class Request {
		private static readonly string[] userAgentList = {
			"Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
			"Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (Linux; Android 5.1.1; Nexus 6 Build/LYZ28E) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
			"Mozilla/5.0 (iPad; CPU OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:46.0) Gecko/20100101 Firefox/46.0",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.1.1 Safari/603.2.4",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:46.0) Gecko/20100101 Firefox/46.0",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/13.10586"
		};

		public static string ChooseUserAgent(string ua) {
			switch (ua) {
			case "mobile":
				return userAgentList[(int)Math.Floor(new Random().NextDouble() * 7)];
			case "pc":
				return userAgentList[(int)Math.Floor(new Random().NextDouble() * 5) + 8];
			default:
				return string.IsNullOrEmpty(ua) ? userAgentList[(int)Math.Floor(new Random().NextDouble() * userAgentList.Length)] : ua;
			}
		}

		public static async Task<(bool, JObject)> CreateRequest(string method, string url, Dictionary<string, object> data, Options options, CookieCollection setCookie) {
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (data is null)
				throw new ArgumentNullException(nameof(data));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			var headers = new Dictionary<string, string> {
				["User-Agent"] = ChooseUserAgent(options.UA),
				["Cookie"] = string.Join("; ", options.Cookie.Cast<Cookie>().Select(t => t.Name + "=" + t.Value))
			};
			if (method.ToUpperInvariant() == "POST")
				headers["Content-Type"] = "application/x-www-form-urlencoded";
			if (url.Contains("music.163.com"))
				headers["Referer"] = "https://music.163.com";
			var data2 = default(Dictionary<string, string>);
			switch (options.Crypto) {
			case "weapi": {
				data["csrf_token"] = options.Cookie.Get("__csrf", string.Empty);
				data2 = Crypto.WEApi(data);
				url = Regex.Replace(url, @"\w*api", "weapi");
				break;
			}
			case "linuxapi": {
				data2 = Crypto.LinuxApi(new Dictionary<string, object> {
					["method"] = method,
					["url"] = Regex.Replace(url, @"\w*api", "api"),
					["params"] = data
				});
				headers["User-Agent"] = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36";
				url = "https://music.163.com/api/linux/forward";
				break;
			}
			case "eapi": {
				var cookie = options.Cookie;
				string csrfToken = cookie.Get("__csrf", string.Empty);
				var header = new Dictionary<string, string>() {
					["osver"] = cookie.Get("osver", string.Empty), // 系统版本
					["deviceId"] = cookie.Get("deviceId", string.Empty), // encrypt.base64.encode(imei + '\t02:00:00:00:00:00\t5106025eb79a5247\t70ffbaac7')
					["appver"] = cookie.Get("appver", "6.1.1"), // app版本
					["versioncode"] = cookie.Get("versioncode", "140"), // 版本号
					["mobilename"] = cookie.Get("mobilename", string.Empty), // 设备model
					["buildver"] = cookie.Get("buildver", GetCurrentTotalSeconds().ToString()),
					["resolution"] = cookie.Get("resolution", "1920x1080"), // 设备分辨率
					["__csrf"] = csrfToken,
					["os"] = cookie.Get("os", "android"),
					["channel"] = cookie.Get("channel", string.Empty),
					["requestId"] = $"{GetCurrentTotalMilliseconds()}_{Math.Floor(new Random().NextDouble() * 1000).ToString().PadLeft(4, '0')}"
				};
				if (!(cookie["MUSIC_U"] is null))
					header["MUSIC_U"] = cookie["MUSIC_U"].Value;
				if (!(cookie["MUSIC_A"] is null))
					header["MUSIC_A"] = cookie["MUSIC_A"].Value;
				headers["Cookie"] = string.Join("; ", header.Select(t => t.Key + "=" + t.Value));
				data["header"] = JsonConvert.SerializeObject(header);
				data2 = Crypto.EApi(options.Url, data);
				url = Regex.Replace(url, @"\w*api", "eapi");
				break;
			}
			}
			try {
				using var handler = new HttpClientHandler {
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
					UseCookies = false
				};
				using var client = new HttpClient(handler);
				using var response = await client.SendAsync(url, method, headers, data2);
				response.EnsureSuccessStatusCode();
				if (response.Headers.TryGetValues("Set-Cookie", out var rawSetCookie)) {
					foreach (string cookie in rawSetCookie)
						setCookie.Add(QuickHttp.ParseCookies(cookie));
				}
				else {
					rawSetCookie = Array.Empty<string>();
				}
				var answer = new JObject {
					["status"] = 500,
					["body"] = null,
					["cookie"] = null
				};
				answer["cookie"] = new JArray(rawSetCookie.Select(x => Regex.Replace(x, @"\s*Domain=[^(;|$)]+;*", string.Empty)).ToList());
				if (options.Crypto == "eapi") {
					byte[] buffer = await response.Content.ReadAsByteArrayAsync();
					try {
						answer["body"] = JObject.Parse(Encoding.UTF8.GetString(Crypto.Decrypt(buffer)));
						var code = (JValue)answer["body"]["code"];
						answer["status"] = code is null ? (int)response.StatusCode : (int)code;
					}
					catch {
						answer["body"] = JObject.Parse(Encoding.UTF8.GetString(buffer));
						answer["status"] = (int)response.StatusCode;
					}
				}
				else {
					answer["body"] = JObject.Parse(await response.Content.ReadAsStringAsync());
					var code = (JValue)answer["body"]["code"];
					answer["status"] = code is null ? (int)response.StatusCode : (int)code;
					if (!(code is null) && (int)code == 502)
						answer["status"] = 200;
				}
				int status = (int)answer["status"];
				status = 100 < status && status < 600 ? status : 400;
				answer["status"] = status;
				return (status == 200, answer);
			}
			catch (Exception ex) {
				return (false, new JObject {
					["status"] = 502,
					["body"] = new JObject {
						["code"] = 502,
						["msg"] = ex.ToFullString()
					},
					["cookie"] = null
				});
			}

			static ulong GetCurrentTotalSeconds() {
				var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
				return (ulong)timeSpan.TotalSeconds;
			}

			static ulong GetCurrentTotalMilliseconds() {
				var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
				return (ulong)timeSpan.TotalMilliseconds;
			}
		}
	}
}
