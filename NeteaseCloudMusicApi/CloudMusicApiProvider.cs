using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using NeteaseCloudMusicApi.util;

namespace NeteaseCloudMusicApi {
	/// <summary>
	/// 参数类型
	/// </summary>
	[Flags]
	public enum ParameterType {
		/// <summary>
		/// 整数（对应类型 <see cref="int"/>）
		/// </summary>
		Integer = 0x0001,
		/// <summary>
		/// 字符串（对应类型 <see cref="string"/>）
		/// </summary>
		String = 0x0002,
		/// <summary>
		/// 整数数组（对应类型 <see cref="int"/>[]）
		/// </summary>
		IntegerArray = 0x0004,
		/// <summary>
		/// 其它
		/// </summary>
		Other = 0x0080
	}

	/// <summary>
	/// 参数级别，指明参数必选/可选/常量
	/// </summary>
	public enum ParameterLevel {
		/// <summary>
		/// 必选参数
		/// </summary>
		Required,
		/// <summary>
		/// 可选参数
		/// </summary>
		Optional,
		/// <summary>
		/// 常量参数
		/// </summary>
		Constant
	}

	/// <summary>
	/// 参数信息
	/// </summary>
	public sealed class ParameterInfo {
		/// <summary>
		/// 参数名
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// 参数类型，可能允许多种类型
		/// </summary>
		public readonly ParameterType Type;
		/// <summary>
		/// 参数级别
		/// </summary>
		public readonly ParameterLevel Level;
		/// <summary>
		/// 默认值（如果为 <see cref="Level"/> 为 <see cref="ParameterLevel.Constant"/>，则 <see cref="DefaultValue"/> 为其对应的常量值，且一定不为 <see langword="null"/>）
		/// </summary>
		public readonly object? DefaultValue;
		internal string? KeyForwarding;
		internal Func<object, object>? Transformer;
		internal Func<Dictionary<string, object>, object>? SpecialHandler;

		internal ParameterInfo(string key) : this(key, ParameterLevel.Required, null) {
		}

		internal ParameterInfo(string key, ParameterLevel level, object? defaultValue) {
			Name = key;
			Level = level;
			DefaultValue = defaultValue;
		}

		internal string GetForwardedKey() {
			return KeyForwarding ?? Name;
		}

		internal object GetRealValue(object value) {
			return Transformer is null ? value : Transformer(value);
		}
	}

	/// <summary>
	/// 网易云音乐API相关信息提供者
	/// </summary>
	public sealed class CloudMusicApiProvider {
		private static readonly IEnumerable<KeyValuePair<string, object>> _emptyQueries = new QueryObjectCollection();

		private readonly string _route;
		private readonly ReadOnlyCollection<ParameterInfo> _parameterInfos;
		private readonly HttpMethod _method;
		private readonly options _options;
		private readonly Func<Dictionary<string, object>, string> _url;
		private readonly string? _batchRoute;
		private Func<Dictionary<string, object>, IEnumerable<KeyValuePair<string, object>>>? _queriesProvider;

		/// <summary>
		/// 对应nodejs版NeteaseCloudMusicApi的Route
		/// </summary>
		public string Route => _route;

		/// <summary>
		/// 参数列表
		/// </summary>
		public IReadOnlyCollection<ParameterInfo> ParameterInfos => _parameterInfos;

		/// <summary>
		/// 用于 <see cref="CloudMusicApiProviders.Batch"/> 接口的Route（不保证当前API一定可用于Batch接口，所以 <see cref="BatchRoute"/> 的返回值不一定有效）
		/// </summary>
		public string? BatchRoute => _batchRoute;

		internal HttpMethod Method => _method;

		internal Func<Dictionary<string, object>, string> Url => _url;

		internal Func<Dictionary<string, object>, IEnumerable<KeyValuePair<string, object>>> Queries => _queriesProvider ?? GetQueries;

		internal options Options => _options;

		internal Func<Dictionary<string, object>, IEnumerable<KeyValuePair<string, object>>>? QueriesProvider {
			get => _queriesProvider;
			set => _queriesProvider = value;
		}

#pragma warning disable CS8618
		internal CloudMusicApiProvider(string route) {
#pragma warning restore CS8618
			if (string.IsNullOrEmpty(route))
				throw new ArgumentNullException(nameof(route));

			_route = route;
		}

		internal CloudMusicApiProvider(string route, string? batchRoute, HttpMethod method, Func<Dictionary<string, object>, string> url, ParameterInfo[] parameterInfos, options options) {
			if (string.IsNullOrEmpty(route))
				throw new ArgumentNullException(nameof(route));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (url is null)
				throw new ArgumentNullException(nameof(url));
			if (parameterInfos is null)
				throw new ArgumentNullException(nameof(parameterInfos));
			if (options is null)
				throw new ArgumentNullException(nameof(options));

			_route = route;
			_batchRoute = batchRoute;
			_method = method;
			_url = url;
			_parameterInfos = new ReadOnlyCollection<ParameterInfo>(parameterInfos);
			_options = options;
		}

		/// <summary>
		/// 获取真实query（可用于Batch接口）
		/// </summary>
		/// <param name="queries"></param>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<string, object>> GetQueries(Dictionary<string, object> queries) {
			QueryObjectCollection data;

			if (_parameterInfos.Count == 0)
				return _emptyQueries;
			data = new QueryObjectCollection();
			foreach (ParameterInfo parameterInfo in _parameterInfos) {
				if (!(parameterInfo.SpecialHandler is null))
					data.Add(parameterInfo.Name, parameterInfo.SpecialHandler(queries));
				else
					switch (parameterInfo.Level) {
					case ParameterLevel.Required:
						data.Add(parameterInfo.Name, parameterInfo.GetRealValue(queries[parameterInfo.GetForwardedKey()]));
						break;
					case ParameterLevel.Optional:
						if (parameterInfo.DefaultValue is null)
							throw new ArgumentNullException(nameof(ParameterInfo.DefaultValue));
						data.Add(parameterInfo.Name, queries.TryGetValue(parameterInfo.GetForwardedKey(), out object value) ? parameterInfo.GetRealValue(value) : parameterInfo.DefaultValue);
						break;
					case ParameterLevel.Constant:
						if (parameterInfo.DefaultValue is null)
							throw new ArgumentNullException(nameof(ParameterInfo.DefaultValue));
						data.Add(parameterInfo.Name, parameterInfo.DefaultValue);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(parameterInfo));
					}
			}
			return data;
		}

		/// <summary />
		public override string ToString() {
			return _route;
		}
	}

	/// <summary>
	/// 已知网易云音乐API相关信息提供者
	/// </summary>
	public static partial class CloudMusicApiProviders {
		private static options BuildOptions(string crypto) {
			return BuildOptions(crypto, null);
		}

		private static options BuildOptions(string crypto, IEnumerable<Cookie>? cookies) {
			return BuildOptions(crypto, cookies, null);
		}

		private static options BuildOptions(string crypto, IEnumerable<Cookie>? cookies, string? ua) {
			return BuildOptions(crypto, cookies, ua, null);
		}

		private static options BuildOptions(string crypto, IEnumerable<Cookie>? cookies, string? ua, string? url) {
			CookieCollection cookieCollection;

			cookieCollection = new CookieCollection();
			if (!(cookies is null))
				foreach (Cookie cookie in cookies)
					cookieCollection.Add(cookie);
			return new options(crypto, cookieCollection, ua, url);
		}

		private static object JsonArrayTransformer(object value) {
			if (value is int)
				return $"[{value}]";
			else if (value is int[])
				return $"[{string.Join(',', ((int[])value).Select(t => t.ToString()))}]";
			else
				throw new ArgumentOutOfRangeException(nameof(value));
		}

		private static object JsonArrayTransformer2(object value) {
			if (value is int)
				return $"[\"{value}\"]";
			else if (value is int[])
				return $"[\"{string.Join(',', ((int[])value).Select(t => t.ToString()))}\"]";
			else
				throw new ArgumentOutOfRangeException(nameof(value));
		}

		private static object BannerTypeTransformer(object type) {
			if (!(type is int))
				throw new ArgumentOutOfRangeException(nameof(type));

			return (int)type switch
			{
				0 => "pc",
				1 => "android",
				2 => "iphone",
				3 => "ipad",
				_ => throw new ArgumentOutOfRangeException(nameof(type)),
			};
		}

		private static object CommentTypeTransformer(object type) {
			if (!(type is int))
				throw new ArgumentOutOfRangeException(nameof(type));

			return (int)type switch
			{
				0 => "R_SO_4_",  // 歌曲
				1 => "R_MV_5_",  // MV
				2 => "A_PL_0_",  // 歌单
				3 => "R_AL_3_",  // 专辑
				4 => "A_DJ_1_",  // 电台
				5 => "R_VI_62_", // 视频
				6 => "A_EV_2_",  // 动态
				_ => throw new ArgumentOutOfRangeException(nameof(type)),
			};
		}

		private static object DjToplistTypeTransformer(object type) {
			if (!(type is string))
				throw new ArgumentOutOfRangeException(nameof(type));

			return (string)type switch
			{
				"new" => 0,
				"hot" => 1,
				_ => throw new ArgumentOutOfRangeException(nameof(type)),
			};
		}

		private static object ResourceTypeTransformer(object type) {
			if (!(type is int))
				throw new ArgumentOutOfRangeException(nameof(type));

			return (int)type switch
			{
				1 => "R_MV_5_",  // MV
				4 => "A_DJ_1_",  // 电台
				5 => "R_VI_62_", // 视频
				6 => "A_EV_2_",  // 动态
				_ => throw new ArgumentOutOfRangeException(nameof(type)),
			};
		}

		private static object TopListIdTransformer(object idx) {
			if (!(idx is int))
				throw new ArgumentOutOfRangeException(nameof(idx));

			return (int)idx switch
			{
				0 => 3779629,     // 云音乐新歌榜
				1 => 3778678,     // 云音乐热歌榜
				2 => 2884035,     // 云音乐原创榜
				3 => 19723756,    // 云音乐飙升榜
				4 => 10520166,    // 云音乐电音榜
				5 => 180106,      // UK排行榜周榜
				6 => 60198,       // 美国Billboard周榜
				7 => 21845217,    // KTV嗨榜
				8 => 11641012,    // iTunes榜
				9 => 120001,      // Hit FM Top榜
				10 => 60131,      // 日本Oricon周榜
				11 => 3733003,    // 韩国Melon排行榜周榜
				12 => 60255,      // 韩国Mnet排行榜周榜
				13 => 46772709,   // 韩国Melon原声周榜
				14 => 112504,     // 中国TOP排行榜(港台榜)
				15 => 64016,      // 中国TOP排行榜(内地榜)
				16 => 10169002,   // 香港电台中文歌曲龙虎榜
				17 => 4395559,    // 华语金曲榜
				18 => 1899724,    // 中国嘻哈榜
				19 => 27135204,   // 法国 NRJ EuroHot 30周榜
				20 => 112463,     // 台湾Hito排行榜
				21 => 3812895,    // Beatport全球电子舞曲榜
				22 => 71385702,   // 云音乐ACG音乐榜
				23 => 991319590,  //云音乐说唱榜
				24 => 71384707,   //云音乐古典音乐榜
				25 => 1978921795, //云音乐电音榜
				26 => 2250011882, //抖音排行榜
				27 => 2617766278, //新声榜
				28 => 745956260,  // 云音乐韩语榜
				29 => 2023401535, // 英国Q杂志中文版周榜
				30 => 2006508653, // 电竞音乐榜
				31 => 2809513713, // 云音乐欧美热歌榜
				32 => 2809577409, // 云音乐欧美新歌榜
				33 => 2847251561, // 说唱TOP榜
				34 => 3001835560, // 云音乐ACG动画榜
				35 => 3001795926, // 云音乐ACG游戏榜
				36 => 3001890046, // 云音乐ACG VOCALOID榜
				_ => throw new ArgumentOutOfRangeException(nameof(idx)),
			};
		}
	}
}
