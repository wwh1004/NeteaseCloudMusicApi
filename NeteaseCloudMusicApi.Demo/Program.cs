using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NeteaseCloudMusicApi.Demo {
	internal static class Program {
		private static async Task Main() {
			try {
				using var api = new CloudMusicApi();

				/******************** 登录 ********************/

				bool isOk;
				JObject json;
				do {
					var queries = new Dictionary<string, string>();
					Console.WriteLine("请输入账号（邮箱或手机）");
					string account = Console.ReadLine();
					bool isPhone = Regex.Match(account, "^[0-9]+$").Success;
					queries[isPhone ? "phone" : "email"] = account;
					Console.WriteLine("请输入密码");
					queries["password"] = Console.ReadLine();
					(isOk, json) = await api.RequestAsync(isPhone ? CloudMusicApiProviders.LoginCellphone : CloudMusicApiProviders.Login, queries);
					if (!isOk)
						Console.WriteLine("登录失败，账号或密码错误");
				} while (!isOk);
				Console.WriteLine("登录成功");
				Console.WriteLine();

				/******************** 登录 ********************/

				/******************** 获取账号信息 ********************/

				(isOk, json) = await api.RequestAsync(CloudMusicApiProviders.LoginStatus, CloudMusicApi.EmptyQueries);
				if (!isOk)
					throw new ApplicationException($"获取账号信息失败： {json}");
				int uid = (int)json["profile"]["userId"];
				Console.WriteLine($"账号ID： {uid}");
				Console.WriteLine($"账号昵称： {json["profile"]["nickname"]}");
				Console.WriteLine();

				/******************** 获取账号信息 ********************/

				/******************** 获取我喜欢的音乐 ********************/

				(isOk, json) = await api.RequestAsync(CloudMusicApiProviders.UserPlaylist, new Dictionary<string, string> { { "uid", uid.ToString() } });
				if (!isOk)
					throw new ApplicationException($"获取用户歌单失败： {json}");
				(isOk, json) = await api.RequestAsync(CloudMusicApiProviders.PlaylistDetail, new Dictionary<string, string> { { "id", json["playlist"][0]["id"].ToString() } });
				if (!isOk)
					throw new ApplicationException($"获取歌单详情失败： {json}");
				var trackIds = json["playlist"]["trackIds"].Select(t => (int)t["id"]).ToArray();
				(isOk, json) = await api.RequestAsync(CloudMusicApiProviders.SongDetail, new Dictionary<string, string> { { "ids", string.Join(",", trackIds) } });
				if (!isOk)
					throw new ApplicationException($"获取歌曲详情失败： {json}");
				Console.WriteLine($"我喜欢的音乐 （{trackIds.Length} 首）：");
				foreach (JObject song in json["songs"])
					Console.WriteLine($"{string.Join(",", song["ar"].Select(t => t["name"]))} - {song["name"]}");
				Console.WriteLine();

				/******************** 获取我喜欢的音乐 ********************/

				/******************** 退出登录 ********************/

				(isOk, json) = await api.RequestAsync(CloudMusicApiProviders.Logout, CloudMusicApi.EmptyQueries);
				if (!isOk)
					throw new ApplicationException($"退出登录失败： {json}");
				Console.WriteLine("退出登录成功");
				Console.WriteLine();

				/******************** 退出登录 ********************/
			}
			catch (Exception ex) {
				Console.WriteLine(ex);
			}
			Console.ReadKey(true);
		}
	}
}
