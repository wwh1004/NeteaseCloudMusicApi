using System;
using System.Net;

namespace NeteaseCloudMusicApi.util {
	internal sealed class options {
		public string crypto;
		public CookieCollection cookie;
		public string? ua;
		public string? url;

		public options(string crypto, CookieCollection cookie, string? ua, string? url) {
			if (crypto is null)
				throw new ArgumentNullException(nameof(crypto));
			if (cookie is null)
				throw new ArgumentNullException(nameof(cookie));

			this.crypto = crypto;
			this.cookie = cookie;
			this.ua = ua;
			this.url = url;
		}
	}
}
