using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WebServerTemplate
{
	public class HttpRequestMessage
	{
		public HttpMethod Method { get; set; }
		public Uri Uri { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		public byte[] Body { get; set; }
	}
}
