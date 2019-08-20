using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Extensions {
	internal static class HttpClientExtensions {
		public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpMethod method, string url) {
			return client.SendAsync(method, url, null, null);
		}

		public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> queries, IEnumerable<KeyValuePair<string, string>> headers) {
			return client.SendAsync(method, url, queries, headers, (byte[])null, "application/x-www-form-urlencoded");
		}

		public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> queries, IEnumerable<KeyValuePair<string, string>> headers, string content, string contentType) {
			return client.SendAsync(method, url, queries, headers, content is null ? null : Encoding.UTF8.GetBytes(content), contentType);
		}

		public static Task<HttpResponseMessage> SendAsync(this HttpClient client, HttpMethod method, string url, IEnumerable<KeyValuePair<string, string>> queries, IEnumerable<KeyValuePair<string, string>> headers, byte[] content, string contentType) {
			if (client is null)
				throw new ArgumentNullException(nameof(client));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (string.IsNullOrEmpty(url))
				throw new ArgumentNullException(nameof(url));
			if (string.IsNullOrEmpty(contentType))
				throw new ArgumentNullException(nameof(contentType));

			UriBuilder uriBuilder;
			HttpRequestMessage request;

			uriBuilder = new UriBuilder(url);
			if (!(queries is null)) {
				string query;

				query = queries.ToQueryString();
				if (!string.IsNullOrEmpty(query))
					if (string.IsNullOrEmpty(uriBuilder.Query))
						uriBuilder.Query = query;
					else
						uriBuilder.Query += "&" + query;
			}
			request = new HttpRequestMessage(method, uriBuilder.Uri);
			if (!(content is null))
				request.Content = new ByteArrayContent(content);
			else if (!(queries is null) && method != HttpMethod.Get)
				request.Content = new FormUrlEncodedContent(queries);
			if (!(request.Content is null))
				request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
			if (!(headers is null))
				foreach (KeyValuePair<string, string> header in headers)
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);
			return client.SendAsync(request);
		}
	}
}
