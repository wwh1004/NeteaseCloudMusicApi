using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using NeteaseCloudMusicApi.Utils;
using Newtonsoft.Json;
using static NeteaseCloudMusicApi.CloudMusicApiProvider;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 所有网易云音乐API提供者
	/// </summary>
	public static class CloudMusicApiProviders {
		/// <summary>
		/// 初始化昵称
		/// </summary>
		public static readonly CloudMusicApiProvider ActivateInitProfile = new CloudMusicApiProvider("/activate/init/profile", HttpMethod.Post, "http://music.163.com/eapi/activate/initProfile", new ParameterInfo[] {
			new ParameterInfo("nickname")
		}, BuildOptions("eapi", null, null, "/api/activate/initProfile"));

		/// <summary>
		/// 获取专辑内容
		/// </summary>
		public static readonly CloudMusicApiProvider Album = new CloudMusicApiProvider("/album", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/album/{q["id"]}", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 专辑动态信息
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumDetailDynamic = new CloudMusicApiProvider("/album/detail/dynamic", HttpMethod.Post, "https://music.163.com/api/album/detail/dynamic", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 最新专辑
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumNewest = new CloudMusicApiProvider("/album/newest", HttpMethod.Post, "https://music.163.com/api/discovery/newAlbum", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 收藏/取消收藏专辑
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumSub = new CloudMusicApiProvider("/album/sub", HttpMethod.Post, q => $"https://music.163.com/api/album/{(q["t"].ToString() == "1" ? "sub" : "unsub")}", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 已收藏专辑列表
		/// </summary>
		public static readonly CloudMusicApiProvider AlbumSublist = new CloudMusicApiProvider("/album/sublist", HttpMethod.Post, "https://music.163.com/weapi/album/sublist", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 25),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 歌手单曲
		/// </summary>
		public static readonly CloudMusicApiProvider Artists = new CloudMusicApiProvider("/artists", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/artist/{q["id"]}", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 歌手专辑列表
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistAlbum = new CloudMusicApiProvider("/artist/album", HttpMethod.Post, q => $"https://music.163.com/weapi/artist/albums/{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, "total")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取歌手描述
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistDesc = new CloudMusicApiProvider("/artist/desc", HttpMethod.Post, "https://music.163.com/weapi/artist/introduction", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 歌手分类列表
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistList = new CloudMusicApiProvider("/artist/list", HttpMethod.Post, "https://music.163.com/weapi/artist/list", new ParameterInfo[] {
			new ParameterInfo("categoryCode", ParameterType.Optional, 1001) { KeyForwarding = "cat" },
			new ParameterInfo("initial", ParameterType.Optional) { Transformer = ArtistListInitialTransformer },
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取歌手 mv
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistMv = new CloudMusicApiProvider("/artist/mv", HttpMethod.Post, "https://music.163.com/weapi/artist/mvs", new ParameterInfo[] {
			new ParameterInfo("artistId") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 收藏/取消收藏歌手
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistSub = new CloudMusicApiProvider("/artist/sub", HttpMethod.Post, q => $"https://music.163.com/weapi/artist/{(q["t"].ToString() == "1" ? "sub" : "unsub")}", new ParameterInfo[] {
			new ParameterInfo("artistId") { KeyForwarding = "id" },
			new ParameterInfo("artistIds") { KeyForwarding = "id", Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 收藏的歌手列表
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistSublist = new CloudMusicApiProvider("/artist/sublist", HttpMethod.Post, "https://music.163.com/weapi/artist/sublist", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 25),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 歌手热门50首歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider ArtistTopSong = new CloudMusicApiProvider("/artist/top/song", HttpMethod.Post, "https://music.163.com/api/artist/top/song", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// banner
		/// </summary>
		public static readonly CloudMusicApiProvider Banner = new CloudMusicApiProvider("/banner", HttpMethod.Post, "https://music.163.com/api/v2/banner/get", new ParameterInfo[] {
			new ParameterInfo("clientType", ParameterType.Optional, "pc") { KeyForwarding = "type", Transformer = BannerTypeTransformer }
		}, BuildOptions("linuxapi"));

		/// <summary>
		/// batch批量请求接口
		/// </summary>
		public static readonly CloudMusicApiProvider Batch = new CloudMusicApiProvider("/batch", HttpMethod.Post, "http://music.163.com/eapi/batch", Array.Empty<ParameterInfo>(), BuildOptions("eapi", null, null, "/api/batch")) {
			DataProvider = queries => {
				var data = new Dictionary<string, object> { ["e_r"] = true };
				foreach (var query in queries) {
					if (query.Key.StartsWith("/api/", StringComparison.Ordinal))
						data.Add(query.Key, query.Value);
				}
				return data;
			}
		};

		/// <summary>
		/// 发送验证码
		/// </summary>
		public static readonly CloudMusicApiProvider CaptchaSent = new CloudMusicApiProvider("/captcha/sent", HttpMethod.Post, "https://music.163.com/weapi/sms/captcha/sent", new ParameterInfo[] {
			new ParameterInfo("cellphone") { KeyForwarding = "phone" },
			new ParameterInfo("ctcode", ParameterType.Optional, 86)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 验证验证码
		/// </summary>
		public static readonly CloudMusicApiProvider CaptchaVerify = new CloudMusicApiProvider("/captcha/verify", HttpMethod.Post, "https://music.163.com/weapi/sms/captcha/verify", new ParameterInfo[] {
			new ParameterInfo("cellphone") { KeyForwarding = "phone" },
			new ParameterInfo("captcha"),
			new ParameterInfo("ctcode", ParameterType.Optional, 86)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 检测手机号码是否已注册
		/// </summary>
		public static readonly CloudMusicApiProvider CellphoneExistenceCheck = new CloudMusicApiProvider("/cellphone/existence/check", HttpMethod.Post, "http://music.163.com/eapi/cellphone/existence/check", new ParameterInfo[] {
			new ParameterInfo("cellphone") { KeyForwarding = "phone" },
			new ParameterInfo("countrycode", ParameterType.Optional, string.Empty)
		}, BuildOptions("eapi", null, null, "/api/cellphone/existence/check"));

		/// <summary>
		/// 音乐是否可用
		/// </summary>
		public static readonly CloudMusicApiProvider CheckMusic = new CloudMusicApiProvider("/check/music", HttpMethod.Post, "https://music.163.com/weapi/song/enhance/player/url", new ParameterInfo[] {
			new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
			new ParameterInfo("br", ParameterType.Optional, 999000)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 发送/删除评论
		/// </summary>
		public static readonly CloudMusicApiProvider Comment = new CloudMusicApiProvider("/comment", HttpMethod.Post, q => $"https://music.163.com/weapi/resource/comments/{(q["t"].ToString() == "1" ? "add" : (q["t"].ToString() == "0" ? "delete" : "reply"))}", Array.Empty<ParameterInfo>(), BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") })) {
			DataProvider = queries => {
				var data = new Dictionary<string, object> {
					["threadId"] = CommentTypeTransformer(queries["type"]).ToString() + queries["id"]
				};
				switch (queries["t"].ToString()) {
				case "0":
					data.Add("commentId", queries["commentId"]);
					break;
				case "1":
					data.Add("content", queries["content"]);
					break;
				case "2":
					data.Add("commentId", queries["commentId"]);
					data.Add("content", queries["content"]);
					break;
				default:
					throw new ArgumentOutOfRangeException("t");
				}
				return data;
			}
		};

		/// <summary>
		/// 专辑评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentAlbum = new CloudMusicApiProvider("/comment/album", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/R_AL_3_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 电台节目评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentDj = new CloudMusicApiProvider("/comment/dj", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/A_DJ_1_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 获取动态评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentEvent = new CloudMusicApiProvider("/comment/event", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/{q["threadId"]}", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 热门评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentHot = new CloudMusicApiProvider("/comment/hot", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/hotcomments/{CommentTypeTransformer(q["type"])}{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 云村热评
		/// </summary>
		public static readonly CloudMusicApiProvider CommentHotwallList = new CloudMusicApiProvider("/comment/hotwall/list", HttpMethod.Post, "https://music.163.com/api/comment/hotwall/list/get", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 给评论点赞
		/// </summary>
		public static readonly CloudMusicApiProvider CommentLike = new CloudMusicApiProvider("/comment/like", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/comment/{(q["t"].ToString() == "1" ? "like" : "unlike")}", new ParameterInfo[] {
			new ParameterInfo("commentId") { KeyForwarding = "cid" },
			new ParameterInfo("threadId", ParameterType.Custom) { CustomHandler = q => q["type"].ToString() == "6" ? q["threadId"] : CommentTypeTransformer(q["type"]).ToString() + q["id"] }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 歌曲评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentMusic = new CloudMusicApiProvider("/comment/music", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/R_SO_4_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// mv 评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentMv = new CloudMusicApiProvider("/comment/mv", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/R_MV_5_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 歌单评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentPlaylist = new CloudMusicApiProvider("/comment/playlist", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/A_PL_0_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 视频评论
		/// </summary>
		public static readonly CloudMusicApiProvider CommentVideo = new CloudMusicApiProvider("/comment/video", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/resource/comments/R_VI_62_{q["id"]}", new ParameterInfo[] {
			new ParameterInfo("rid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("beforeTime", ParameterType.Optional, 0) { KeyForwarding = "before" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 签到
		/// </summary>
		public static readonly CloudMusicApiProvider DailySignin = new CloudMusicApiProvider("/daily_signin", HttpMethod.Post, "https://music.163.com/weapi/point/dailyTask", new ParameterInfo[] {
			new ParameterInfo("type", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 我的数字专辑
		/// </summary>
		public static readonly CloudMusicApiProvider DigitalAlbumPurchased = new CloudMusicApiProvider("/digitalAlbum/purchased", HttpMethod.Post, "https://music.163.com/api/digitalAlbum/purchased", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台banner
		/// </summary>
		public static readonly CloudMusicApiProvider DjBanner = new CloudMusicApiProvider("/dj/banner", HttpMethod.Post, "http://music.163.com/weapi/djradio/banner/get", Array.Empty<ParameterInfo>(), BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 电台 - 非热门类型
		/// </summary>
		public static readonly CloudMusicApiProvider DjCategoryExcludehot = new CloudMusicApiProvider("/dj/category/excludehot", HttpMethod.Post, "http://music.163.com/weapi/djradio/category/excludehot", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 推荐类型
		/// </summary>
		public static readonly CloudMusicApiProvider DjCategoryRecommend = new CloudMusicApiProvider("/dj/category/recommend", HttpMethod.Post, "http://music.163.com/weapi/djradio/home/category/recommend", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 分类
		/// </summary>
		public static readonly CloudMusicApiProvider DjCatelist = new CloudMusicApiProvider("/dj/catelist", HttpMethod.Post, "https://music.163.com/weapi/djradio/category/get", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 详情
		/// </summary>
		public static readonly CloudMusicApiProvider DjDetail = new CloudMusicApiProvider("/dj/detail", HttpMethod.Post, "https://music.163.com/weapi/djradio/get", new ParameterInfo[] {
			new ParameterInfo("id") { KeyForwarding = "rid" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 热门电台
		/// </summary>
		public static readonly CloudMusicApiProvider DjHot = new CloudMusicApiProvider("/dj/hot", HttpMethod.Post, "https://music.163.com/weapi/djradio/hot/v1", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 付费精选
		/// </summary>
		public static readonly CloudMusicApiProvider DjPaygift = new CloudMusicApiProvider("/dj/paygift", HttpMethod.Post, "https://music.163.com/weapi/djradio/home/paygift/list?_nmclfl=1", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 节目
		/// </summary>
		public static readonly CloudMusicApiProvider DjProgram = new CloudMusicApiProvider("/dj/program", HttpMethod.Post, "https://music.163.com/weapi/dj/program/byradio", new ParameterInfo[] {
			new ParameterInfo("radioId") { KeyForwarding = "rid" },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("asc", ParameterType.Optional, false)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 节目详情
		/// </summary>
		public static readonly CloudMusicApiProvider DjProgramDetail = new CloudMusicApiProvider("/dj/program/detail", HttpMethod.Post, "https://music.163.com/weapi/dj/program/detail", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 节目榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjProgramToplist = new CloudMusicApiProvider("/dj/program/toplist", HttpMethod.Post, "https://music.163.com/api/program/toplist/v1", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 24小时节目榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjProgramToplistHours = new CloudMusicApiProvider("/dj/program/toplist/hours", HttpMethod.Post, "https://music.163.com/api/program/toplist/hours", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 类别热门电台
		/// </summary>
		public static readonly CloudMusicApiProvider DjRadioHot = new CloudMusicApiProvider("/dj/radio/hot", HttpMethod.Post, "https://music.163.com/api/djradio/hot", new ParameterInfo[] {
			new ParameterInfo("cateId"),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 推荐
		/// </summary>
		public static readonly CloudMusicApiProvider DjRecommend = new CloudMusicApiProvider("/dj/recommend", HttpMethod.Post, "https://music.163.com/weapi/djradio/recommend/v1", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 分类推荐
		/// </summary>
		public static readonly CloudMusicApiProvider DjRecommendType = new CloudMusicApiProvider("/dj/recommend/type", HttpMethod.Post, "https://music.163.com/weapi/djradio/recommend", new ParameterInfo[] {
			new ParameterInfo("cateId") { KeyForwarding = "type" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 订阅
		/// </summary>
		public static readonly CloudMusicApiProvider DjSub = new CloudMusicApiProvider("/dj/sub", HttpMethod.Post, q => $"https://music.163.com/weapi/djradio/{(q["t"].ToString() == "1" ? "sub" : "unsub")}", new ParameterInfo[] {
			new ParameterInfo("id") { KeyForwarding = "rid" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台的订阅列表
		/// </summary>
		public static readonly CloudMusicApiProvider DjSublist = new CloudMusicApiProvider("/dj/sublist", HttpMethod.Post, "https://music.163.com/weapi/djradio/get/subed", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 今日优选
		/// </summary>
		public static readonly CloudMusicApiProvider DjTodayPerfered = new CloudMusicApiProvider("/dj/today/perfered", HttpMethod.Post, "http://music.163.com/weapi/djradio/home/today/perfered", new ParameterInfo[] {
			new ParameterInfo("page", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 新晋电台榜/热门电台榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjToplist = new CloudMusicApiProvider("/dj/toplist", HttpMethod.Post, "https://music.163.com/api/djradio/toplist", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("type", ParameterType.Optional, "new") { Transformer = DjToplistTypeTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 24小时主播榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjToplistHours = new CloudMusicApiProvider("/dj/toplist/hours", HttpMethod.Post, "https://music.163.com/api/dj/toplist/hours", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 主播新人榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjToplistNewcomer = new CloudMusicApiProvider("/dj/toplist/newcomer", HttpMethod.Post, "https://music.163.com/api/dj/toplist/newcomer", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 付费精品
		/// </summary>
		public static readonly CloudMusicApiProvider DjToplistPay = new CloudMusicApiProvider("/dj/toplist/pay", HttpMethod.Post, "https://music.163.com/api/djradio/toplist/pay", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 电台 - 最热主播榜
		/// </summary>
		public static readonly CloudMusicApiProvider DjToplistPopular = new CloudMusicApiProvider("/dj/toplist/popular", HttpMethod.Post, "https://music.163.com/api/dj/toplist/popular", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 100)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取动态消息
		/// </summary>
		public static readonly CloudMusicApiProvider Event = new CloudMusicApiProvider("/event", HttpMethod.Post, "https://music.163.com/weapi/v1/event/get", new ParameterInfo[] {
			new ParameterInfo("pagesize", ParameterType.Optional, 20),
			new ParameterInfo("lasttime", ParameterType.Optional, -1)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 删除用户动态
		/// </summary>
		public static readonly CloudMusicApiProvider EventDel = new CloudMusicApiProvider("/event/del", HttpMethod.Post, "https://music.163.com/eapi/event/delete", new ParameterInfo[] {
			new ParameterInfo("id") { KeyForwarding = "evId" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 转发用户动态
		/// </summary>
		public static readonly CloudMusicApiProvider EventForward = new CloudMusicApiProvider("/event/forward", HttpMethod.Post, "https://music.163.com/weapi/event/forward", new ParameterInfo[] {
			new ParameterInfo("forwards"),
			new ParameterInfo("id") { KeyForwarding = "evId" },
			new ParameterInfo("eventUserId") { KeyForwarding = "uid" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 垃圾桶
		/// </summary>
		public static readonly CloudMusicApiProvider FmTrash = new CloudMusicApiProvider("/fm_trash", HttpMethod.Post, q => $"https://music.163.com/weapi/radio/trash/add?alg=RT&songId={q["id"]}&time={q.GetValueOrDefault("time", "25")}", new ParameterInfo[] {
			new ParameterInfo("songId") { KeyForwarding = "id" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 关注/取消关注用户
		/// </summary>
		public static readonly CloudMusicApiProvider Follow = new CloudMusicApiProvider("/follow", HttpMethod.Post, q => $"https://music.163.com/weapi/user/{(q["t"].ToString() == "1" ? "follow" : "delfollow")}/{q["id"]}", Array.Empty<ParameterInfo>(), BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 获取热门话题
		/// </summary>
		public static readonly CloudMusicApiProvider HotTopic = new CloudMusicApiProvider("/hot/topic", HttpMethod.Post, "http://music.163.com/weapi/act/hot", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 喜欢音乐
		/// </summary>
		public static readonly CloudMusicApiProvider Like = new CloudMusicApiProvider("/like", HttpMethod.Post, q => $"https://music.163.com/weapi/radio/like?alg={q.GetValueOrDefault("alg", "itembased")}&trackId={q["id"]}&time={q.GetValueOrDefault("time", "25")}", new ParameterInfo[] {
			new ParameterInfo("trackId") { KeyForwarding = "id" },
			new ParameterInfo("like")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 喜欢音乐列表
		/// </summary>
		public static readonly CloudMusicApiProvider Likelist = new CloudMusicApiProvider("/likelist", HttpMethod.Post, "https://music.163.com/weapi/song/like/get", new ParameterInfo[] {
			new ParameterInfo("uid")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 邮箱登录
		/// </summary>
		public static readonly CloudMusicApiProvider Login = new CloudMusicApiProvider("/login", HttpMethod.Post, "https://music.163.com/weapi/login", new ParameterInfo[] {
			new ParameterInfo("username") { KeyForwarding = "email" },
			new ParameterInfo("password") { Transformer = t => t.ToString().ToByteArrayUtf8().ComputeMd5().ToHexStringLower() },
			new ParameterInfo("rememberLogin", ParameterType.Constant, true),
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }, "pc"));

		/// <summary>
		/// 手机登录
		/// </summary>
		public static readonly CloudMusicApiProvider LoginCellphone = new CloudMusicApiProvider("/login/cellphone", HttpMethod.Post, "https://music.163.com/weapi/login/cellphone", new ParameterInfo[] {
			new ParameterInfo("phone"),
			new ParameterInfo("countrycode", ParameterType.Optional, string.Empty),
			new ParameterInfo("password") { Transformer = t => t.ToString().ToByteArrayUtf8().ComputeMd5().ToHexStringLower() },
			new ParameterInfo("rememberLogin", ParameterType.Constant, true)
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }, "pc"));

		/// <summary>
		/// 登录刷新
		/// </summary>
		public static readonly CloudMusicApiProvider LoginRefresh = new CloudMusicApiProvider("/login/refresh", HttpMethod.Post, "https://music.163.com/weapi/login/token/refresh", Array.Empty<ParameterInfo>(), BuildOptions("weapi", null, "pc"));

		/// <summary>
		/// 登录状态
		/// </summary>
		public static readonly CloudMusicApiProvider LoginStatus = new CloudMusicApiProvider("/login/status");

		/// <summary>
		/// 退出登录
		/// </summary>
		public static readonly CloudMusicApiProvider Logout = new CloudMusicApiProvider("/logout", HttpMethod.Post, "https://music.163.com/weapi/logout", Array.Empty<ParameterInfo>(), BuildOptions("weapi", null, "pc"));

		/// <summary>
		/// 歌词
		/// </summary>
		public static readonly CloudMusicApiProvider Lyric = new CloudMusicApiProvider("/lyric", HttpMethod.Post, "https://music.163.com/weapi/song/lyric?lv=-1&kv=-1&tv=-1", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("linuxapi"));

		/// <summary>
		/// 通知 - 评论
		/// </summary>
		public static readonly CloudMusicApiProvider MsgComments = new CloudMusicApiProvider("/msg/comments", HttpMethod.Post, q => $"https://music.163.com/api/v1/user/comments/{q["uid"]}", new ParameterInfo[] {
			new ParameterInfo("uid"),
			new ParameterInfo("beforeTime", ParameterType.Optional, -1) { KeyForwarding = "before" },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 通知 - @我
		/// </summary>
		public static readonly CloudMusicApiProvider MsgForwards = new CloudMusicApiProvider("/msg/forwards", HttpMethod.Post, "https://music.163.com/api/forwards/get", new ParameterInfo[] {
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 通知 - 通知
		/// </summary>
		public static readonly CloudMusicApiProvider MsgNotices = new CloudMusicApiProvider("/msg/notices", HttpMethod.Post, "https://music.163.com/api/msg/notices", new ParameterInfo[] {
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 通知 - 私信
		/// </summary>
		public static readonly CloudMusicApiProvider MsgPrivate = new CloudMusicApiProvider("/msg/private", HttpMethod.Post, "https://music.163.com/api/msg/private/users", new ParameterInfo[] {
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 私信内容
		/// </summary>
		public static readonly CloudMusicApiProvider MsgPrivateHistory = new CloudMusicApiProvider("/msg/private/history", HttpMethod.Post, "https://music.163.com/api/msg/private/history", new ParameterInfo[] {
			new ParameterInfo("userId") { KeyForwarding = "uid" },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("time", ParameterType.Optional, 0) { KeyForwarding = "before" },
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 全部 mv
		/// </summary>
		public static readonly CloudMusicApiProvider MvAll = new CloudMusicApiProvider("/mv/all", HttpMethod.Post, "https://interface.music.163.com/api/mv/all", new ParameterInfo[] {
			new ParameterInfo("tags", ParameterType.Custom) { CustomHandler = q => JsonConvert.SerializeObject(new Dictionary<string, object> {
				["地区"] = q.GetValueOrDefault("area", "全部"),
				["类型"] = q.GetValueOrDefault("type", "全部"),
				["排序"] = q.GetValueOrDefault("order", "上升最快")
			}) },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取 mv 数据
		/// </summary>
		public static readonly CloudMusicApiProvider MvDetail = new CloudMusicApiProvider("/mv/detail", HttpMethod.Post, "https://music.163.com/weapi/mv/detail", new ParameterInfo[] {
			new ParameterInfo("id") { KeyForwarding = "mvid" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 网易出品mv
		/// </summary>
		public static readonly CloudMusicApiProvider MvExclusiveRcmd = new CloudMusicApiProvider("/mv/exclusive/rcmd", HttpMethod.Post, "https://interface.music.163.com/api/mv/exclusive/rcmd", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 最新 mv
		/// </summary>
		public static readonly CloudMusicApiProvider MvFirst = new CloudMusicApiProvider("/mv/first", HttpMethod.Post, "https://interface.music.163.com/weapi/mv/first", new ParameterInfo[] {
			new ParameterInfo("area", ParameterType.Optional, string.Empty),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 收藏/取消收藏 MV
		/// </summary>
		public static readonly CloudMusicApiProvider MvSub = new CloudMusicApiProvider("/mv/sub", HttpMethod.Post, q => $"https://music.163.com/weapi/mv/{(q["t"].ToString() == "1" ? "sub" : "unsub")}", new ParameterInfo[] {
			new ParameterInfo("mvId") { KeyForwarding = "mvid" },
			new ParameterInfo("mvIds") { KeyForwarding = "mvid", Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 收藏的 MV 列表
		/// </summary>
		public static readonly CloudMusicApiProvider MvSublist = new CloudMusicApiProvider("/mv/sublist", HttpMethod.Post, "https://music.163.com/weapi/cloudvideo/allvideo/sublist", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 25),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// mv 地址
		/// </summary>
		public static readonly CloudMusicApiProvider MvUrl = new CloudMusicApiProvider("/mv/url", HttpMethod.Post, "https://music.163.com/weapi/song/enhance/play/mv/url", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("r", ParameterType.Optional, 1080) { KeyForwarding = "res" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 推荐歌单
		/// </summary>
		public static readonly CloudMusicApiProvider Personalized = new CloudMusicApiProvider("/personalized", HttpMethod.Post, "https://music.163.com/weapi/personalized/playlist", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, true),
			new ParameterInfo("n", ParameterType.Constant, "1000")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 推荐电台
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalizedDjprogram = new CloudMusicApiProvider("/personalized/djprogram", HttpMethod.Post, "https://music.163.com/weapi/personalized/djprogram", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 推荐 mv
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalizedMv = new CloudMusicApiProvider("/personalized/mv", HttpMethod.Post, "https://music.163.com/weapi/personalized/mv", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 推荐新音乐
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalizedNewsong = new CloudMusicApiProvider("/personalized/newsong", HttpMethod.Post, "https://music.163.com/weapi/personalized/newsong", new ParameterInfo[] {
			new ParameterInfo("type", ParameterType.Constant, "recommend")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 独家放送
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalizedPrivatecontent = new CloudMusicApiProvider("/personalized/privatecontent", HttpMethod.Post, "https://music.163.com/weapi/personalized/privatecontent", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 私人 FM
		/// </summary>
		public static readonly CloudMusicApiProvider PersonalFm = new CloudMusicApiProvider("/personal_fm", HttpMethod.Post, "https://music.163.com/weapi/v1/radio/get", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 歌单分类
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistCatlist = new CloudMusicApiProvider("/playlist/catlist", HttpMethod.Post, "https://music.163.com/weapi/playlist/catalogue", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 新建歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistCreate = new CloudMusicApiProvider("/playlist/create", HttpMethod.Post, "https://music.163.com/weapi/playlist/create", new ParameterInfo[] {
			new ParameterInfo("name"),
			new ParameterInfo("privacy", ParameterType.Optional, string.Empty)
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 删除歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDelete = new CloudMusicApiProvider("/playlist/delete", HttpMethod.Post, "https://music.163.com/weapi/playlist/delete", new ParameterInfo[] {
			new ParameterInfo("pid") { KeyForwarding = "id" }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 更新歌单描述
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDescUpdate = new CloudMusicApiProvider("/playlist/desc/update", HttpMethod.Post, "http://interface3.music.163.com/eapi/playlist/desc/update", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("desc")
		}, BuildOptions("eapi", null, null, "/api/playlist/desc/update"));

		/// <summary>
		/// 获取歌单详情
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistDetail = new CloudMusicApiProvider("/playlist/detail", HttpMethod.Post, "https://music.163.com/weapi/v3/playlist/detail", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("n", ParameterType.Constant, 100000),
			new ParameterInfo("s", ParameterType.Optional, 8)
		}, BuildOptions("linuxapi"));

		/// <summary>
		/// 热门歌单分类
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistHot = new CloudMusicApiProvider("/playlist/hot", HttpMethod.Post, "https://music.163.com/weapi/playlist/hottags", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 更新歌单名
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistNameUpdate = new CloudMusicApiProvider("/playlist/name/update", HttpMethod.Post, "http://interface3.music.163.com/eapi/playlist/update/name", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("name")
		}, BuildOptions("eapi", null, null, "/api/playlist/update/name"));

		/// <summary>
		/// 收藏/取消收藏歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistSubscribe = new CloudMusicApiProvider("/playlist/subscribe", HttpMethod.Post, q => $"https://music.163.com/weapi/playlist/{(q["t"].ToString() == "1" ? "subscribe" : "unsubscribe")}", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 歌单收藏者
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistSubscribers = new CloudMusicApiProvider("/playlist/subscribers", HttpMethod.Post, "https://music.163.com/weapi/playlist/subscribers", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("limit", ParameterType.Optional, 20),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 更新歌单标签
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistTagsUpdate = new CloudMusicApiProvider("/playlist/tags/update", HttpMethod.Post, "http://interface3.music.163.com/eapi/playlist/tags/update", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("tags")
		}, BuildOptions("eapi", null, null, "/api/playlist/tags/update"));

		/// <summary>
		/// 对歌单添加或删除歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistTracks = new CloudMusicApiProvider("/playlist/tracks", HttpMethod.Post, "https://music.163.com/weapi/playlist/manipulate/tracks", new ParameterInfo[] {
			new ParameterInfo("op"),
			new ParameterInfo("pid"),
			new ParameterInfo("trackIds") { Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 更新歌单
		/// </summary>
		public static readonly CloudMusicApiProvider PlaylistUpdate = new CloudMusicApiProvider("/playlist/update", HttpMethod.Post, "https://music.163.com/weapi/batch", Array.Empty<ParameterInfo>(), BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") })) {
			DataProvider = queries => new Dictionary<string, object> {
				["/api/playlist/update/name"] = $"{{\"id\":{queries["id"]},\"name\":\"{queries["name"]}\"}}",
				["/api/playlist/desc/update"] = $"{{\"id\":{queries["id"]},\"desc\":\"{queries["desc"]}\"}}",
				["/api/playlist/tags/update"] = $"{{\"id\":{queries["id"]},\"tags\":\"{queries["tags"]}\"}}"
			}
		};

		/// <summary>
		/// 心动模式/智能播放
		/// </summary>
		public static readonly CloudMusicApiProvider PlaymodeIntelligenceList = new CloudMusicApiProvider("/playmode/intelligence/list", HttpMethod.Post, "http://music.163.com/weapi/playmode/intelligence/list", new ParameterInfo[] {
			new ParameterInfo("songId") { KeyForwarding = "id" },
			new ParameterInfo("playlistId") { KeyForwarding = "pid" },
			new ParameterInfo("startMusicId", ParameterType.Custom) { CustomHandler = q => q.TryGetValue("sid", out object sid) ? sid : q["id"] },
			new ParameterInfo("count", ParameterType.Optional, 1),
			new ParameterInfo("type", ParameterType.Constant, "fromPlayOne")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 推荐节目
		/// </summary>
		public static readonly CloudMusicApiProvider ProgramRecommend = new CloudMusicApiProvider("/program/recommend", HttpMethod.Post, "https://music.163.com/weapi/program/recommend/v1", new ParameterInfo[] {
			new ParameterInfo("cateId", ParameterType.Optional, string.Empty) { KeyForwarding = "type" },
			new ParameterInfo("limit", ParameterType.Optional, 10),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 更换绑定手机
		/// </summary>
		public static readonly CloudMusicApiProvider Rebind = new CloudMusicApiProvider("/rebind", HttpMethod.Post, "https://music.163.com/api/user/replaceCellphone", new ParameterInfo[] {
			new ParameterInfo("captcha"),
			new ParameterInfo("phone"),
			new ParameterInfo("oldcaptcha"),
			new ParameterInfo("ctcode", ParameterType.Optional, 86)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 每日推荐歌单
		/// </summary>
		public static readonly CloudMusicApiProvider RecommendResource = new CloudMusicApiProvider("/recommend/resource", HttpMethod.Post, "https://music.163.com/weapi/v1/discovery/recommend/resource", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 每日推荐歌曲
		/// </summary>
		public static readonly CloudMusicApiProvider RecommendSongs = new CloudMusicApiProvider("/recommend/songs", HttpMethod.Post, "https://music.163.com/weapi/v1/discovery/recommend/songs", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 注册(修改密码)
		/// </summary>
		public static readonly CloudMusicApiProvider RegisterCellphone = new CloudMusicApiProvider("/register/cellphone", HttpMethod.Post, "https://music.163.com/weapi/register/cellphone", new ParameterInfo[] {
			new ParameterInfo("captcha"),
			new ParameterInfo("phone"),
			new ParameterInfo("password") { Transformer = t => t.ToString().ToByteArrayUtf8().ComputeMd5().ToHexStringLower() },
			new ParameterInfo("nickname")
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 相关视频
		/// </summary>
		public static readonly CloudMusicApiProvider RelatedAllvideo = new CloudMusicApiProvider("/related/allvideo", HttpMethod.Post, "https://music.163.com/weapi/cloudvideo/v1/allvideo/rcmd", new ParameterInfo[] {
			new ParameterInfo("id"),
			new ParameterInfo("type") { KeyForwarding = "id", Transformer = t => Regex.IsMatch(t.ToString(), @"^\d+$") ? 0 : 1 }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 相关歌单推荐
		/// </summary>
		public static readonly CloudMusicApiProvider RelatedPlaylist = new CloudMusicApiProvider("/related/playlist");

		/// <summary>
		/// 资源点赞( MV,电台,视频)
		/// </summary>
		public static readonly CloudMusicApiProvider ResourceLike = new CloudMusicApiProvider("/resource/like", HttpMethod.Post, q => $"https://music.163.com/weapi/resource/{(q["t"].ToString() == "1" ? "like" : "unlike")}", new ParameterInfo[] {
			new ParameterInfo("threadId", ParameterType.Custom) { CustomHandler = q => q["type"].ToString() == "6" ? q["threadId"] : ResourceTypeTransformer(q["type"]).ToString() + q["id"] }
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 听歌打卡
		/// </summary>
		public static readonly CloudMusicApiProvider Scrobble = new CloudMusicApiProvider("/scrobble", HttpMethod.Post, "https://music.163.com/weapi/feedback/weblog", new ParameterInfo[] {
			new ParameterInfo("logs", ParameterType.Custom) { CustomHandler = q => JsonConvert.SerializeObject(new Dictionary<string, object> {
				["action"] = "play",
				["json"] = JsonConvert.SerializeObject(new Dictionary<string, object> {
					["id"] = q["id"],
					["sourceId"] = q["sourceId"],
					["time"] = q["time"],
					["download"] = 0,
					["end"] = "playend",
					["type"] = "song",
					["wifi"] = 0
				})
			}) }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 搜索
		/// </summary>
		public static readonly CloudMusicApiProvider Search = new CloudMusicApiProvider("/search", HttpMethod.Post, "https://music.163.com/weapi/search/get", new ParameterInfo[] {
			new ParameterInfo("s") { KeyForwarding = "keywords" },
			new ParameterInfo("type", ParameterType.Optional, 1),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 默认搜索关键词
		/// </summary>
		public static readonly CloudMusicApiProvider SearchDefault = new CloudMusicApiProvider("/search/default", HttpMethod.Post, "http://interface3.music.163.com/eapi/search/defaultkeyword/get", Array.Empty<ParameterInfo>(), BuildOptions("eapi", null, null, "/api/search/defaultkeyword/get"));

		/// <summary>
		/// 热搜列表(简略)
		/// </summary>
		public static readonly CloudMusicApiProvider SearchHot = new CloudMusicApiProvider("/search/hot", HttpMethod.Post, "https://music.163.com/weapi/search/hot", new ParameterInfo[] {
			new ParameterInfo("type", ParameterType.Constant, 1111)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 热搜列表(详细)
		/// </summary>
		public static readonly CloudMusicApiProvider SearchHotDetail = new CloudMusicApiProvider("/search/hot/detail", HttpMethod.Post, "https://music.163.com/weapi/hotsearchlist/get", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 搜索多重匹配
		/// </summary>
		public static readonly CloudMusicApiProvider SearchMultimatch = new CloudMusicApiProvider("/search/multimatch", HttpMethod.Post, "https://music.163.com/weapi/search/suggest/multimatch", new ParameterInfo[] {
			new ParameterInfo("s") { KeyForwarding = "keywords" },
			new ParameterInfo("type", ParameterType.Optional, 1)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 搜索建议
		/// </summary>
		public static readonly CloudMusicApiProvider SearchSuggest = new CloudMusicApiProvider("/search/suggest", HttpMethod.Post, q => $"https://music.163.com/weapi/search/suggest/{(q.GetValueOrDefault("type", null).ToString() == "mobile" ? "keyword" : "web")}", new ParameterInfo[] {
			new ParameterInfo("s") { KeyForwarding = "keywords" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 发送私信(带歌单)
		/// </summary>
		public static readonly CloudMusicApiProvider SendPlaylist = new CloudMusicApiProvider("/send/playlist", HttpMethod.Post, "https://music.163.com/weapi/msg/private/send", new ParameterInfo[] {
			new ParameterInfo("userIds") { KeyForwarding = "user_ids", Transformer = JsonArrayTransformer },
			new ParameterInfo("msg"),
			new ParameterInfo("id", ParameterType.Optional, string.Empty) { KeyForwarding = "playlist" },
			new ParameterInfo("type", ParameterType.Constant, "playlist")
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 发送私信
		/// </summary>
		public static readonly CloudMusicApiProvider SendText = new CloudMusicApiProvider("/send/text", HttpMethod.Post, "https://music.163.com/weapi/msg/private/send", new ParameterInfo[] {
			new ParameterInfo("userIds") { KeyForwarding = "user_ids", Transformer = JsonArrayTransformer },
			new ParameterInfo("msg"),
			new ParameterInfo("id", ParameterType.Optional, string.Empty) { KeyForwarding = "playlist" },
			new ParameterInfo("type", ParameterType.Constant, "text")
		}, BuildOptions("weapi", new Cookie[] { new Cookie("os", "pc") }));

		/// <summary>
		/// 设置
		/// </summary>
		public static readonly CloudMusicApiProvider Setting = new CloudMusicApiProvider("/setting", HttpMethod.Post, "https://music.163.com/api/user/setting", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 分享歌曲、歌单、mv、电台、电台节目到动态
		/// </summary>
		public static readonly CloudMusicApiProvider ShareResource = new CloudMusicApiProvider("/share/resource", HttpMethod.Post, "http://music.163.com/weapi/share/friends/resource", new ParameterInfo[] {
			new ParameterInfo("type", ParameterType.Optional, "song"),
			new ParameterInfo("msg", ParameterType.Optional, string.Empty),
			new ParameterInfo("id", ParameterType.Optional, string.Empty)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取相似歌手
		/// </summary>
		public static readonly CloudMusicApiProvider SimiArtist = new CloudMusicApiProvider("/simi/artist", HttpMethod.Post, "https://music.163.com/weapi/discovery/simiArtist", new ParameterInfo[] {
			new ParameterInfo("artistid") { KeyForwarding = "id" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 相似 mv
		/// </summary>
		public static readonly CloudMusicApiProvider SimiMv = new CloudMusicApiProvider("/simi/mv", HttpMethod.Post, "https://music.163.com/weapi/discovery/simiMV", new ParameterInfo[] {
			new ParameterInfo("mvid")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取相似歌单
		/// </summary>
		public static readonly CloudMusicApiProvider SimiPlaylist = new CloudMusicApiProvider("/simi/playlist", HttpMethod.Post, "https://music.163.com/weapi/discovery/simiPlaylist", new ParameterInfo[] {
			new ParameterInfo("songid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取相似音乐
		/// </summary>
		public static readonly CloudMusicApiProvider SimiSong = new CloudMusicApiProvider("/simi/song", HttpMethod.Post, "https://music.163.com/weapi/v1/discovery/simiSong", new ParameterInfo[] {
			new ParameterInfo("songid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取最近 5 个听了这首歌的用户
		/// </summary>
		public static readonly CloudMusicApiProvider SimiUser = new CloudMusicApiProvider("/simi/user", HttpMethod.Post, "https://music.163.com/weapi/discovery/simiUser", new ParameterInfo[] {
			new ParameterInfo("songid") { KeyForwarding = "id" },
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取歌曲详情
		/// </summary>
		public static readonly CloudMusicApiProvider SongDetail = new CloudMusicApiProvider("/song/detail", HttpMethod.Post, "https://music.163.com/weapi/v3/song/detail", new ParameterInfo[] {
			new ParameterInfo("c") { KeyForwarding = "ids", Transformer = t => "[" + string.Join(",", t.ToString().Split(',').Select(m => "{\"id\":" + m.Trim() + "}")) + "]" },
			new ParameterInfo("ids") { Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取音乐 url
		/// </summary>
		public static readonly CloudMusicApiProvider SongUrl = new CloudMusicApiProvider("/song/url", HttpMethod.Post, "https://music.163.com/api/song/enhance/player/url", new ParameterInfo[] {
			new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
			new ParameterInfo("br", ParameterType.Optional, 999000)
		}, BuildOptions("linuxapi", new Cookie[] { new Cookie("os", "pc"), new Cookie("_ntes_nuid", new Random().RandomBytes(16).ToHexStringLower()) }));

		/// <summary>
		/// 所有榜单
		/// </summary>
		public static readonly CloudMusicApiProvider Toplist = new CloudMusicApiProvider("/toplist", HttpMethod.Post, "https://music.163.com/weapi/toplist", Array.Empty<ParameterInfo>(), BuildOptions("linuxapi"));

		/// <summary>
		/// 歌手榜
		/// </summary>
		public static readonly CloudMusicApiProvider ToplistArtist = new CloudMusicApiProvider("/toplist/artist", HttpMethod.Post, "https://music.163.com/weapi/toplist/artist", new ParameterInfo[] {
			new ParameterInfo("type", ParameterType.Constant, 1),
			new ParameterInfo("limit", ParameterType.Constant, 100),
			new ParameterInfo("offset", ParameterType.Constant, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 所有榜单内容摘要
		/// </summary>
		public static readonly CloudMusicApiProvider ToplistDetail = new CloudMusicApiProvider("/toplist/detail", HttpMethod.Post, "https://music.163.com/weapi/toplist/detail", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 新碟上架
		/// </summary>
		public static readonly CloudMusicApiProvider TopAlbum = new CloudMusicApiProvider("/top/album", HttpMethod.Post, "https://music.163.com/weapi/album/new", new ParameterInfo[] {
			new ParameterInfo("area", ParameterType.Optional, "ALL") { KeyForwarding = "type" },
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 热门歌手
		/// </summary>
		public static readonly CloudMusicApiProvider TopArtists = new CloudMusicApiProvider("/top/artists", HttpMethod.Post, "https://music.163.com/weapi/artist/top", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 排行榜
		/// </summary>
		public static readonly CloudMusicApiProvider Top_List = new CloudMusicApiProvider("/top/list", HttpMethod.Post, "https://music.163.com/weapi/v3/playlist/detail", new ParameterInfo[] {
			new ParameterInfo("id") { KeyForwarding = "idx", Transformer = TopListIdTransformer },
			new ParameterInfo("n", ParameterType.Constant, 10000)
		}, BuildOptions("linuxapi"));

		/// <summary>
		/// mv 排行
		/// </summary>
		public static readonly CloudMusicApiProvider TopMv = new CloudMusicApiProvider("/top/mv", HttpMethod.Post, "https://music.163.com/weapi/mv/toplist", new ParameterInfo[] {
			new ParameterInfo("area", ParameterType.Optional, string.Empty),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 歌单 ( 网友精选碟 )
		/// </summary>
		public static readonly CloudMusicApiProvider TopPlaylist = new CloudMusicApiProvider("/top/playlist", HttpMethod.Post, "https://music.163.com/weapi/playlist/list", new ParameterInfo[] {
			new ParameterInfo("cat", ParameterType.Optional, "全部"),
			new ParameterInfo("order", ParameterType.Optional, "hot"),
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取精品歌单
		/// </summary>
		public static readonly CloudMusicApiProvider TopPlaylistHighquality = new CloudMusicApiProvider("/top/playlist/highquality", HttpMethod.Post, "https://music.163.com/weapi/playlist/highquality/list", new ParameterInfo[] {
			new ParameterInfo("cat", ParameterType.Optional, "全部"),
			new ParameterInfo("limit", ParameterType.Optional, 50),
			new ParameterInfo("lasttime", ParameterType.Optional, 0) { KeyForwarding = "before" },
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 新歌速递
		/// </summary>
		public static readonly CloudMusicApiProvider TopSong = new CloudMusicApiProvider("/top/song", HttpMethod.Post, "https://music.163.com/weapi/v1/discovery/new/songs", new ParameterInfo[] {
			new ParameterInfo("areaId") { KeyForwarding = "type" },
			new ParameterInfo("total", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 用户电台
		/// </summary>
		public static readonly CloudMusicApiProvider UserAudio = new CloudMusicApiProvider("/user/audio", HttpMethod.Post, "https://music.163.com/weapi/djradio/get/byuser", new ParameterInfo[] {
			new ParameterInfo("userId") { KeyForwarding = "uid" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 云盘
		/// </summary>
		public static readonly CloudMusicApiProvider UserCloud = new CloudMusicApiProvider("/user/cloud", HttpMethod.Post, "https://music.163.com/weapi/v1/cloud/get", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 云盘歌曲删除
		/// </summary>
		public static readonly CloudMusicApiProvider UserCloudDel = new CloudMusicApiProvider("/user/cloud/del", HttpMethod.Post, "http://music.163.com/weapi/cloud/del", new ParameterInfo[] {
			new ParameterInfo("songIds") { KeyForwarding = "id", Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 云盘数据详情
		/// </summary>
		public static readonly CloudMusicApiProvider UserCloudDetail = new CloudMusicApiProvider("/user/cloud/detail", HttpMethod.Post, "https://music.163.com/weapi/v1/cloud/get/byids", new ParameterInfo[] {
			new ParameterInfo("songIds") { KeyForwarding = "id", Transformer = JsonArrayTransformer }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户详情
		/// </summary>
		public static readonly CloudMusicApiProvider UserDetail = new CloudMusicApiProvider("/user/detail", HttpMethod.Post, q => $"https://music.163.com/weapi/v1/user/detail/{q["uid"]}", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 获取用户电台
		/// </summary>
		public static readonly CloudMusicApiProvider UserDj = new CloudMusicApiProvider("/user/dj", HttpMethod.Post, q => $"https://music.163.com/weapi/dj/program/{q["uid"]}", new ParameterInfo[] {
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户动态
		/// </summary>
		public static readonly CloudMusicApiProvider UserEvent = new CloudMusicApiProvider("/user/event", HttpMethod.Post, q => $"https://music.163.com/weapi/event/get/{q["uid"]}", new ParameterInfo[] {
			new ParameterInfo("getcounts", ParameterType.Constant, true),
			new ParameterInfo("time", ParameterType.Optional, -1) { KeyForwarding = "lasttime" },
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("total", ParameterType.Constant, false)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户粉丝列表
		/// </summary>
		public static readonly CloudMusicApiProvider UserFolloweds = new CloudMusicApiProvider("/user/followeds", HttpMethod.Post, q => $"https://music.163.com/eapi/user/getfolloweds/{q["uid"]}", new ParameterInfo[] {
			new ParameterInfo("userId") { KeyForwarding = "uid" },
			new ParameterInfo("time", ParameterType.Optional, -1) { KeyForwarding = "lasttime" },
			new ParameterInfo("limit", ParameterType.Optional, 30)
		}, BuildOptions("eapi", null, null, "/api/user/getfolloweds"));

		/// <summary>
		/// 获取用户关注列表
		/// </summary>
		public static readonly CloudMusicApiProvider UserFollows = new CloudMusicApiProvider("/user/follows", HttpMethod.Post, q => $"https://music.163.com/weapi/user/getfollows/{q["uid"]}", new ParameterInfo[] {
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("order", ParameterType.Constant, true)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户歌单
		/// </summary>
		public static readonly CloudMusicApiProvider UserPlaylist = new CloudMusicApiProvider("/user/playlist", HttpMethod.Post, "https://music.163.com/weapi/user/playlist", new ParameterInfo[] {
			new ParameterInfo("uid"),
			new ParameterInfo("limit", ParameterType.Optional, 30),
			new ParameterInfo("offset", ParameterType.Optional, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户播放记录
		/// </summary>
		public static readonly CloudMusicApiProvider UserRecord = new CloudMusicApiProvider("/user/record", HttpMethod.Post, "https://music.163.com/weapi/v1/play/record", new ParameterInfo[] {
			new ParameterInfo("uid"),
			new ParameterInfo("type", ParameterType.Optional, 1)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取用户信息 , 歌单，收藏，mv, dj 数量
		/// </summary>
		public static readonly CloudMusicApiProvider UserSubcount = new CloudMusicApiProvider("/user/subcount", HttpMethod.Post, "https://music.163.com/weapi/subcount", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 更新用户信息
		/// </summary>
		public static readonly CloudMusicApiProvider UserUpdate = new CloudMusicApiProvider("/user/update", HttpMethod.Post, "https://music.163.com/weapi/user/profile/update", new ParameterInfo[] {
			new ParameterInfo("birthday"),
			new ParameterInfo("city"),
			new ParameterInfo("gender"),
			new ParameterInfo("nickname"),
			new ParameterInfo("province"),
			new ParameterInfo("signature"),
			new ParameterInfo("avatarImgId", ParameterType.Constant, 0)
		}, BuildOptions("weapi"));

		/// <summary>
		/// 视频详情
		/// </summary>
		public static readonly CloudMusicApiProvider VideoDetail = new CloudMusicApiProvider("/video/detail", HttpMethod.Post, "https://music.163.com/weapi/cloudvideo/v1/video/detail", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取视频标签下的视频
		/// </summary>
		public static readonly CloudMusicApiProvider VideoGroup = new CloudMusicApiProvider("/video/group", HttpMethod.Post, "https://music.163.com/weapi/videotimeline/videogroup/get", new ParameterInfo[] {
			new ParameterInfo("groupId") { KeyForwarding = "id" },
			new ParameterInfo("offset", ParameterType.Optional, 0),
			new ParameterInfo("needUrl", ParameterType.Constant, true),
			new ParameterInfo("resolution", ParameterType.Optional, 1080) { KeyForwarding = "res" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取视频标签列表
		/// </summary>
		public static readonly CloudMusicApiProvider VideoGroupList = new CloudMusicApiProvider("/video/group/list", HttpMethod.Post, "https://music.163.com/api/cloudvideo/group/list", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		/// <summary>
		/// 收藏视频
		/// </summary>
		public static readonly CloudMusicApiProvider VideoSub = new CloudMusicApiProvider("/video/sub", HttpMethod.Post, q => $"https://music.163.com/weapi/cloudvideo/video/{(q["t"].ToString() == "1" ? "sub" : "unsub")}", new ParameterInfo[] {
			new ParameterInfo("id")
		}, BuildOptions("weapi"));

		/// <summary>
		/// 获取视频播放地址
		/// </summary>
		public static readonly CloudMusicApiProvider VideoUrl = new CloudMusicApiProvider("/video/url", HttpMethod.Post, "https://music.163.com/weapi/cloudvideo/playurl", new ParameterInfo[] {
			new ParameterInfo("ids") { KeyForwarding = "id", Transformer = JsonArrayTransformer },
			new ParameterInfo("resolution", ParameterType.Optional, 1080) { KeyForwarding = "res" }
		}, BuildOptions("weapi"));

		/// <summary>
		/// 操作记录 （无文档）
		/// </summary>
		public static readonly CloudMusicApiProvider Weblog = new CloudMusicApiProvider("/weblog", HttpMethod.Post, "https://music.163.com/weapi/feedback/weblog", Array.Empty<ParameterInfo>(), BuildOptions("weapi"));

		private static Options BuildOptions(string crypto) {
			return BuildOptions(crypto, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies) {
			return BuildOptions(crypto, cookies, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies, string ua) {
			return BuildOptions(crypto, cookies, ua, null);
		}

		private static Options BuildOptions(string crypto, IEnumerable<Cookie> cookies, string ua, string url) {
			var cookies2 = new CookieCollection();
			if (!(cookies is null)) {
				foreach (var cookie in cookies)
					cookies2.Add(cookie);
			}
			var options = new Options {
				Crypto = crypto,
				Cookie = cookies2,
				UA = ua,
				Url = url
			};
			return options;
		}

		private static object ArtistListInitialTransformer(object value) {
			if (value is null)
				return null;
			if (value is string s)
				return (int)char.ToUpperInvariant(s[0]);
			if (value is char c)
				return (int)char.ToUpperInvariant(c);
			var typeCode = Type.GetTypeCode(value.GetType());
			if (TypeCode.SByte <= typeCode && typeCode <= TypeCode.UInt64)
				return value;
			throw new ArgumentOutOfRangeException(nameof(value));
		}

		private static object JsonArrayTransformer(object value) {
			return "[" + value is string s ? s.Replace(" ", string.Empty) : value + "]";
		}

		private static object BannerTypeTransformer(object type) {
			switch (type.ToString()) {
			case "0": return "pc";
			case "1": return "android";
			case "2": return "iphone";
			case "3": return "ipad";
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object CommentTypeTransformer(object type) {
			switch (type.ToString()) {
			case "0": return "R_SO_4_";  // 歌曲
			case "1": return "R_MV_5_";  // MV
			case "2": return "A_PL_0_";  // 歌单
			case "3": return "R_AL_3_";  // 专辑
			case "4": return "A_DJ_1_";  // 电台
			case "5": return "R_VI_62_"; // 视频
			case "6": return "A_EV_2_";  // 动态
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object DjToplistTypeTransformer(object type) {
			switch (type.ToString()) {
			case "new": return "0";
			case "hot": return "1";
			default:
				throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object ResourceTypeTransformer(object type) {
			switch (type.ToString()) {
			case "1": return "R_MV_5_";  // MV
			case "4": return "A_DJ_1_";  // 电台
			case "5": return "R_VI_62_"; // 视频
			case "6": return "A_EV_2_";  // 动态
			default: throw new ArgumentOutOfRangeException(nameof(type));
			}
		}

		private static object TopListIdTransformer(object idx) {
			switch (idx.ToString()) {
			case "0": return 3779629;     // 云音乐新歌榜
			case "1": return 3778678;     // 云音乐热歌榜
			case "2": return 2884035;     // 云音乐原创榜
			case "3": return 19723756;    // 云音乐飙升榜
			case "4": return 10520166;    // 云音乐电音榜
			case "5": return 180106;      // UK排行榜周榜
			case "6": return 60198;       // 美国Billboard周榜
			case "7": return 21845217;    // KTV嗨榜
			case "8": return 11641012;    // iTunes榜
			case "9": return 120001;      // Hit FM Top榜
			case "10": return 60131;      // 日本Oricon周榜
			case "11": return 3733003;    // 韩国Melon排行榜周榜
			case "12": return 60255;      // 韩国Mnet排行榜周榜
			case "13": return 46772709;   // 韩国Melon原声周榜
			case "14": return 112504;     // 中国TOP排行榜(港台榜)
			case "15": return 64016;      // 中国TOP排行榜(内地榜)
			case "16": return 10169002;   // 香港电台中文歌曲龙虎榜
			case "17": return 4395559;    // 华语金曲榜
			case "18": return 1899724;    // 中国嘻哈榜
			case "19": return 27135204;   // 法国 NRJ EuroHot 30周榜
			case "20": return 112463;     // 台湾Hito排行榜
			case "21": return 3812895;    // Beatport全球电子舞曲榜
			case "22": return 71385702;   // 云音乐ACG音乐榜
			case "23": return 991319590;  // 云音乐说唱榜
			case "24": return 71384707;   // 云音乐古典音乐榜
			case "25": return 1978921795; // 云音乐电音榜
			case "26": return 2250011882; // 抖音排行榜
			case "27": return 2617766278; // 新声榜
			case "28": return 745956260;  // 云音乐韩语榜
			case "29": return 2023401535; // 英国Q杂志中文版周榜
			case "30": return 2006508653; // 电竞音乐榜
			case "31": return 2809513713; // 云音乐欧美热歌榜
			case "32": return 2809577409; // 云音乐欧美新歌榜
			case "33": return 2847251561; // 说唱TOP榜
			case "34": return 3001835560; // 云音乐ACG动画榜
			case "35": return 3001795926; // 云音乐ACG游戏榜
			case "36": return 3001890046; // 云音乐ACG VOCALOID榜
			default: throw new ArgumentOutOfRangeException(nameof(idx));
			}
		}
	}
}
