using System.Collections.Generic;
using System.Linq;

namespace NeteaseCloudMusicApi {
	internal sealed class QueryCollection : List<KeyValuePair<string, string>> {
		public QueryCollection() {
		}

		public QueryCollection(IEnumerable<KeyValuePair<string, string>> collection) : base(collection) {
		}

		public void Add(string key, string value) {
			Add(new KeyValuePair<string, string>(key, value));
		}
	}

	internal sealed class QueryObjectCollection : List<KeyValuePair<string, object>> {
		public void Add(string key, object value) {
			Add(new KeyValuePair<string, object>(key, value));
		}

		public static explicit operator QueryCollection(QueryObjectCollection value) {
			return new QueryCollection(value.Select(t => new KeyValuePair<string, string>(t.Key, t.Value.ToString())));
		}
	}
}
