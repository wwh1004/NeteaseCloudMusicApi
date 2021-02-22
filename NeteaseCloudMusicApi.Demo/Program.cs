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
				var api = new CloudMusicApi();

				/******************** 登录 ********************/

				while (true) {
					var queries = new Dictionary<string, object>();
					Console.WriteLine("请输入账号（邮箱或手机）");
					string account = Console.ReadLine();
					bool isPhone = Regex.Match(account, "^[0-9]+$").Success;
					queries[isPhone ? "phone" : "email"] = account;
					Console.WriteLine("请输入密码");
					queries["password"] = Console.ReadLine();
					if (!CloudMusicApi.IsSuccess(await api.RequestAsync(isPhone ? CloudMusicApiProviders.LoginCellphone : CloudMusicApiProviders.Login, queries, false)))
						Console.WriteLine("登录失败，账号或密码错误");
					else
						break;
				}
				Console.WriteLine("登录成功");
				Console.WriteLine();

				/******************** 登录 ********************/

				/******************** 获取账号信息 ********************/

				var json = await api.RequestAsync(CloudMusicApiProviders.LoginStatus);
				long uid = (long)json["profile"]["userId"];
				Console.WriteLine($"账号ID： {uid}");
				Console.WriteLine($"账号昵称： {json["profile"]["nickname"]}");
				Console.WriteLine();

				/******************** 获取账号信息 ********************/

				/******************** 获取我喜欢的音乐 ********************/

				json = await api.RequestAsync(CloudMusicApiProviders.UserPlaylist, new Dictionary<string, object> { ["uid"] = uid });
				json = await api.RequestAsync(CloudMusicApiProviders.PlaylistDetail, new Dictionary<string, object> { ["id"] = json["playlist"][0]["id"] });
				int[] trackIds = json["playlist"]["trackIds"].Select(t => (int)t["id"]).ToArray();
				json = await api.RequestAsync(CloudMusicApiProviders.SongDetail, new Dictionary<string, object> { ["ids"] = trackIds });
				Console.WriteLine($"我喜欢的音乐（{trackIds.Length} 首）：");
				foreach (var song in json["songs"])
					Console.WriteLine($"{string.Join(",", song["ar"].Select(t => t["name"]))} - {song["name"]}");
				Console.WriteLine();

				/******************** 获取我喜欢的音乐 ********************/

				/******************** 获取我的关注 ********************/

				/******************** 获取我的关注 ********************/

				json = await api.RequestAsync(CloudMusicApiProviders.UserFollows, new Dictionary<string, object> { ["uid"] = uid });
				Console.WriteLine($"我的关注：");
				foreach (var user in json["follow"])
					Console.WriteLine(user["nickname"]);
				Console.WriteLine();

				/******************** 获取我的动态 ********************/

				json = await api.RequestAsync(CloudMusicApiProviders.UserEvent, new Dictionary<string, object> { ["uid"] = uid });
				Console.WriteLine($"我的动态：");
				foreach (var @event in json["events"])
					Console.WriteLine(JObject.Parse((string)@event["json"])["msg"]);
				Console.WriteLine();

				/******************** 获取我的动态 ********************/

				/******************** 退出登录 ********************/

				json = await api.RequestAsync(CloudMusicApiProviders.Logout);
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
