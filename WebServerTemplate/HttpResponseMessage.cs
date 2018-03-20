using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace WebServerTemplate
{
	public class HttpResponseMessage
	{
		public HttpStatusCode StatusCode { get; set; }
		public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

		public byte[] Body { get; set; }

		public void SerializeToStream(Stream s)
		{
			using (var sw = new StreamWriter(s, Encoding.ASCII, 1024, true))
			{
				sw.WriteLine(string.Format("HTTP/1.1 {0} {1}", (int)StatusCode, StatusCode));
				foreach (var kv in Headers)
				{
					sw.WriteLine(string.Format("{0}: {1}", kv.Key, kv.Value));
				}
				sw.WriteLine();
			}
			if (Body != null)
			{
				s.Write(Body, 0, Body.Length);
			}
		}
	}
}
