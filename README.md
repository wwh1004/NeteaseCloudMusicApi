# NeteaseCloudMusicApi
C#版 网易云音乐 API

## 简介
本项目翻译自Node.js项目[Binaryify/NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)

更新与原项目同步

使用方式请参考[原项目文档](https://binaryify.github.io/NeteaseCloudMusicApi)，参数与返回结果与原项目完全一致

本项目需要 .NET Standard 2.0 （.NET Framework 4.6.1+ / .NET Core 2.0+） ，可跨平台使用

## Dll与Demo下载
GitHub:

[NeteaseCloudMusicApi-net472.zip（.NET Framework版Demo）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi-net472.zip)

[NeteaseCloudMusicApi-netcoreapp2.1.zip（.NET Core版Demo）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi-netcoreapp2.1.zip)

[NeteaseCloudMusicApi-netstandard2.0.zip（已编译Dll）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi-netstandard2.0.zip)

AppVeyor: [![Build status](https://ci.appveyor.com/api/projects/status/guu6sx3yyy5a846o?svg=true)](https://ci.appveyor.com/project/wwh1004/neteasecloudmusicapi)

## 功能特性

1. 登录
2. 刷新登录
3. 发送验证码
4. 校验验证码
5. 注册(修改密码)
6. 获取用户信息 , 歌单，收藏，mv, dj 数量
7. 获取用户歌单
8. 获取用户电台
9. 获取用户关注列表
10. 获取用户粉丝列表
11. 获取用户动态
12. 获取用户播放记录
13. 获取精品歌单
14. 获取歌单详情
15. 搜索
16. 搜索建议
17. 获取歌词
18. 歌曲评论
19. 收藏单曲到歌单
20. 专辑评论
21. 歌单评论
22. mv 评论
23. 电台节目评论
24. banner
25. 获取歌曲详情
26. 获取专辑内容
27. 获取歌手单曲
28. 获取歌手 mv
29. 获取歌手专辑
30. 获取歌手描述
31. 获取相似歌手
32. 获取相似歌单
33. 相似 mv
34. 获取相似音乐
35. 获取最近 5 个听了这首歌的用户
36. 获取每日推荐歌单
37. 获取每日推荐歌曲
38. 私人 FM
39. 签到
40. 喜欢音乐
41. 垃圾桶
42. 歌单 ( 网友精选碟 )
43. 新碟上架
44. 热门歌手
45. 最新 mv
46. 推荐 mv
47. 推荐歌单
48. 推荐新音乐
49. 推荐电台
50. 推荐节目
51. 独家放送
52. mv 排行
53. 获取 mv 数据
54. 播放 mv/视频
55. 排行榜
56. 歌手榜
57. 云盘
58. 电台 - 推荐
59. 电台 - 分类
60. 电台 - 分类推荐
61. 电台 - 订阅
62. 电台 - 详情
63. 电台 - 节目
64. 给评论点赞
65. 获取动态
66. 热搜列表(简略)
67. 发送私信
68. 发送私信歌单
69. 新建歌单
70. 收藏/取消收藏歌单
71. 歌单分类
72. 收藏的歌手列表
73. 订阅的电台列表
74. 相关歌单推荐
75. 付费精选接口
76. 音乐是否可用检查接口
77. 登录状态
78. 获取视频播放地址
79. 发送/删除评论
80. 热门评论
81. 视频评论
82. 退出登录
83. 所有榜单
84. 所有榜单内容摘要
85. 收藏视频
86. 收藏 MV
87. 视频详情
88. 相关视频
89. 关注用户
90. 新歌速递
91. 喜欢音乐列表(无序)
92. 收藏的 MV 列表
93. 获取最新专辑
94. 听歌打卡
95. 获取视频标签下的视频
96. 已收藏专辑列表
97. 获取动态评论
98. 歌单收藏者列表
99. 云盘歌曲删除
100. 热门话题
101. 电台 - 推荐类型
102. 电台 - 非热门类型
103. 电台 - 今日优选
104. 心动模式/智能播放
105. 转发动态
106. 删除动态
107. 分享歌曲、歌单、mv、电台、电台节目到动态
108. 通知-私信
109. 通知-评论
110. 通知-@我
111. 通知-通知
112. 设置
113. 云盘数据详情
114. 私信内容
115. 我的数字专辑
116. batch批量请求接口
117. 获取视频标签列表
118. 全部mv
119. 网易出品mv
120. 收藏/取消收藏专辑
121. 专辑动态信息
122. 热搜列表(详细)
123. 更换绑定手机
124. 检测手机号码是否已注册
125. 初始化昵称
126. 更新歌单描述
127. 更新歌单名
128. 更新歌单标签
129. 默认搜索关键词
130. 删除歌单
131. 电台banner
132. 用户电台
133. 热门电台
134. 电台 - 节目详情

## 样例
### 实例化API
``` csharp
using (CloudMusicApi api = new CloudMusicApi()) {
	// code here
}
```

### 完整样例-显示"我喜欢的音乐"
``` csharp
using (CloudMusicApi api = new CloudMusicApi()) {
	try {
		bool isOk;
		JObject json;
		int uid;
		int[] trackIds;

		/******************** 登录 ********************/

		do {
			Dictionary<string, string> queries;
			string account;
			bool isPhone;

			queries = new Dictionary<string, string>();
			Console.WriteLine("请输入账号（邮箱或手机）");
			account = Console.ReadLine();
			isPhone = Regex.Match(account, "^[0-9]+$").Success;
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
		uid = (int)json["profile"]["userId"];
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
		trackIds = json["playlist"]["trackIds"].Select(t => (int)t["id"]).ToArray();
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
}
```
