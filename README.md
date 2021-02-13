# NeteaseCloudMusicApi [![Build status](https://ci.appveyor.com/api/projects/status/guu6sx3yyy5a846o?svg=true)](https://ci.appveyor.com/project/wwh1004/neteasecloudmusicapi) [![NuGet](https://img.shields.io/nuget/v/NeteaseCloudMusicApi.svg)](https://www.nuget.org/packages/NeteaseCloudMusicApi)
C#版 网易云音乐 API

## 简介
本项目翻译自Node.js项目[Binaryify/NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)

更新与原项目同步

使用方式请参考[原项目文档](https://binaryify.github.io/NeteaseCloudMusicApi)，参数与返回结果与原项目完全一致

本项目需要 .NET Standard 2.0 （.NET Framework 4.6.1+ / .NET Core 2.0+） ，可跨平台使用

### 样例项目

[wwh1004/NLyric](https://github.com/wwh1004/NLyric) - 使用本项目自动搜索下载歌词

## Dll与Demo下载
GitHub:

[NeteaseCloudMusicApi-netstandard2.0.zip（已编译Dll）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi-netstandard2.0.zip)

[NeteaseCloudMusicApi.Demo-net472.zip（.NET Framework版Demo）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi.Demo-net472.zip)

[NeteaseCloudMusicApi.Demo-netcoreapp2.1.zip（.NET Core版Demo）](https://github.com/wwh1004/NeteaseCloudMusicApi/releases/latest/download/NeteaseCloudMusicApi.Demo-netcoreapp2.1.zip)

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

135. 电台 - 节目榜

136. 电台 - 新晋电台榜/热门电台榜

137. 类别热门电台

138. 云村热评

139. 电台24小时节目榜

140. 电台24小时主播榜

141. 电台最热主播榜

142. 电台主播新人榜

143. 电台付费精品榜

144. 歌手热门50首歌曲

145. 购买数字专辑

146. 获取 mv 点赞转发评论数数据

147. 获取视频点赞转发评论数数据

148. 调整歌单顺序

149. 调整歌曲顺序

150. 独家放送列表

151. 获取推荐视频

152. 获取视频分类列表 

153. 获取全部视频列表接口

154. 获取历史日推可用日期列表

155. 获取历史日推详细数据

156. 国家编码列表

157. 首页-发现

158. 首页-发现-圆形图标入口列表 

159. 全部新碟

160. 数字专辑-新碟上架

161. 数字专辑&数字单曲-榜单

162. 数字专辑-语种风格馆

163. 数字专辑详情

164. 更新头像

165. 歌单封面上传 (未完成)

166. 楼层评论

167. 歌手全部歌曲

168. 精品歌单标签列表

169. 用户等级信息

170. 电台个性推荐

171. 用户绑定信息

172. 用户绑定手机

173. 新版评论

174. 点赞过的视频

175. 收藏视频到视频歌单

176. 删除视频歌单里的视频

177. 最近播放的视频

178. 音乐日历

179. 电台订阅者列表

180. 云贝签到信息

181. 云贝签到

182. 云贝所有任务

183. 云贝todo任务

184. 云贝今日签到信息

185. 云贝完成任务

186. 云贝收入

187. 云贝支出

188. 云贝账户信息

189. 账号信息

190. 最近联系人

191. 私信音乐

192. 抱一抱评论

193. 评论抱一抱列表

194. 收藏的专栏

195. 关注歌手新歌

196. 关注歌手新MV

197. 歌手详情

198. 云盘上传 (未完成)

199. 二维码登录

200. 话题详情

201. 话题详情热门动态

202. 歌单详情动态

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
