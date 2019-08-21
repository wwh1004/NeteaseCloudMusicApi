using System;
using System.Collections.Generic;
using System.Extensions;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusicApi.util {
	internal static class request {
		private static readonly string[] userAgentList = {
			"Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1",
			"Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (Linux; Android 5.1.1; Nexus 6 Build/LYZ28E) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Mobile Safari/537.36",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Mobile/14F89;GameHelper",
			"Mozilla/5.0 (iPhone; CPU iPhone OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
			"Mozilla/5.0 (iPad; CPU OS 10_0 like Mac OS X) AppleWebKit/602.1.38 (KHTML, like Gecko) Version/10.0 Mobile/14A300 Safari/602.1",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10.12; rv:46.0) Gecko/20100101 Firefox/46.0",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36",
			"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.1.1 Safari/603.2.4",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:46.0) Gecko/20100101 Firefox/46.0",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/13.10586"
		};

		public static string chooseUserAgent(string ua) {
			switch (ua) {
			case "mobile":
				return userAgentList[(int)Math.Floor(new Random().NextDouble() * 7)];
			case "pc":
				return userAgentList[(int)Math.Floor(new Random().NextDouble() * 5) + 8];
			default:
				return string.IsNullOrEmpty(ua) ? userAgentList[(int)Math.Floor(new Random().NextDouble() * userAgentList.Length)] : ua;
			}
		}

		public static async Task<(bool, JObject)> createRequest(HttpClient client, HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> data_, options options) {
			if (client is null)
				throw new ArgumentNullException(nameof(client));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (data_ is null)
				throw new ArgumentNullException(nameof(data_));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			Dictionary<string, string> headers;
			Dictionary<string, string> data;
			JObject answer;
			HttpResponseMessage response;

			headers = new Dictionary<string, string> {
				["User-Agent"] = chooseUserAgent(options.ua),
				["Cookie"] = string.Join("; ", options.cookie.Cast<Cookie>().Select(t => Uri.EscapeDataString(t.Name) + "=" + Uri.EscapeDataString(t.Value)))
			};
			if (method == HttpMethod.Post)
				headers["Content-Type"] = "application/x-www-form-urlencoded";
			if (url.Contains("music.163.com"))
				headers["Referer"] = "https://music.163.com";
			data = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in data_)
				data.Add(item.Key, item.Value);
			switch (options.crypto) {
			case "weapi": {
					data["csrf_token"] = options.cookie["__csrf"]?.Value ?? string.Empty;
					data = crypto.weapi(data);
					url = Regex.Replace(url, @"\w*api", "weapi");
					break;
				}
			case "linuxapi": {
					data = crypto.linuxapi(new Dictionary<string, object> {
						{ "method", method.Method },
						{ "url", Regex.Replace(url, @"\w*api", "api") },
						{ "params", data }
					});
					headers["User-Agent"] = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.90 Safari/537.36";
					url = "https://music.163.com/api/linux/forward";
					break;
				}
			case "eapi": {
					CookieCollection cookie;
					string csrfToken;
					Dictionary<string, string> header;

					cookie = new CookieCollection();
					foreach (Cookie item in options.cookie)
						cookie.Add(new Cookie(item.Name, item.Value));
					csrfToken = cookie["__csrf"]?.Value ?? string.Empty;
					header = new Dictionary<string, string>() {
						{ "osver", cookie["osver"]?.Value ?? string.Empty }, // 系统版本
						{ "deviceId", cookie["deviceId"]?.Value ?? string.Empty }, // encrypt.base64.encode(imei + '\t02:00:00:00:00:00\t5106025eb79a5247\t70ffbaac7')
						{ "appver", cookie["appver"]?.Value ?? "6.1.1" }, // app版本
						{ "versioncode",  cookie["versioncode"]?.Value ?? "140" }, // 版本号
						{ "mobilename", cookie["mobilename"]?.Value ?? string.Empty }, // 设备model
						{ "buildver", cookie["buildver"]?.Value ?? GetCurrentTotalSeconds().ToString() },
						{ "resolution", cookie["resolution"]?.Value ?? "1920x1080" }, // 设备分辨率
						{ "__csrf", csrfToken },
						{ "os", cookie["os"]?.Value ?? "android" },
						{ "channel", cookie["channel"]?.Value ?? string.Empty },
						{ "requestId", $"{GetCurrentTotalMilliseconds()}_{Math.Floor(new Random().NextDouble() * 1000).ToString().PadLeft(4, '0')}" }
					};
					if (!(cookie["MUSIC_U"] is null))
						header["MUSIC_U"] = cookie["MUSIC_U"].Value;
					if (!(cookie["MUSIC_A"] is null))
						header["MUSIC_A"] = cookie["MUSIC_A"].Value;
					headers["Cookie"] = string.Join("; ", header.Select(t => Uri.EscapeDataString(t.Key) + "=" + Uri.EscapeDataString(t.Value)));
					data["header"] = JsonConvert.SerializeObject(header);
					data = crypto.eapi(options.url, data);
					url = Regex.Replace(url, @"\w*api", "eapi");
					break;
				}
			}
			answer = new JObject {
				{ "status", 500 },
				{ "body", null },
				{ "cookie", null }
			};
			response = null;
			try {
				IEnumerable<string> temp1;
				JValue temp2;
				int temp3;

				response = await client.SendAsync(method, url, null, headers, data.ToQueryString(), "application/x-www-form-urlencoded");
				if (!response.IsSuccessStatusCode)
					throw new HttpRequestException();
				if (!response.Headers.TryGetValues("set-cookie", out temp1))
					temp1 = Array.Empty<string>();
				answer["cookie"] = new JArray(temp1.Select(x => Regex.Replace(x, @"\s*Domain=[^(;|$)]+;*", string.Empty)).Where(x => !string.IsNullOrEmpty(x)).ToList());
				if (options.crypto == "eapi") {
					DeflateStream stream;
					byte[] buffer;

					stream = null;
					try {
						stream = new DeflateStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
						buffer = ReadStream(stream);
					}
					catch {
						buffer = await response.Content.ReadAsByteArrayAsync();
					}
					finally {
						stream?.Dispose();
					}
					try {
						answer["body"] = JObject.Parse(Encoding.UTF8.GetString(crypto.decrypt(buffer)));
						temp2 = (JValue)answer["body"]["code"];
						answer["status"] = temp2 is null ? (int)response.StatusCode : (int)temp2;
					}
					catch {
						answer["body"] = JObject.Parse(Encoding.UTF8.GetString(buffer));
						answer["status"] = (int)response.StatusCode;
					}
				}
				else {
					answer["body"] = JObject.Parse(await response.Content.ReadAsStringAsync());
					temp2 = (JValue)answer["body"]["code"];
					answer["status"] = temp2 is null ? (int)response.StatusCode : (int)temp2;
				}
				temp3 = (int)answer["status"];
				temp3 = 100 < temp3 && temp3 < 600 ? temp3 : 400;
				answer["status"] = temp3;
				return (temp3 == 200, answer);
			}
			catch (Exception ex) {
				answer["status"] = 502;
				answer["body"] = new JObject {
					{ "code", 502 },
					{ "msg", ex.ToFullString() }
				};
				return (false, answer);
			}
			finally {
				response?.Dispose();
			}

			ulong GetCurrentTotalSeconds() {
				TimeSpan _timeSpan;

				_timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
				return (ulong)_timeSpan.TotalSeconds;
			}

			ulong GetCurrentTotalMilliseconds() {
				TimeSpan _timeSpan;

				_timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
				return (ulong)_timeSpan.TotalMilliseconds;
			}

			byte[] ReadStream(Stream _stream) {
				byte[] _buffer;
				List<byte> _byteList;

				_buffer = new byte[0x1000];
				_byteList = new List<byte>();
				for (int i = 0; i < int.MaxValue; i++) {
					int count;

					count = _stream.Read(_buffer, 0, _buffer.Length);
					if (count == 0x1000)
						_byteList.AddRange(_buffer);
					else if (count == 0)
						return _byteList.ToArray();
					else
						for (int j = 0; j < count; j++)
							_byteList.Add(_buffer[j]);
				}
				throw new OutOfMemoryException();
			}
		}
	}
}
