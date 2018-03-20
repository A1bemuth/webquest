using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Http;
using System.Net;

namespace WebServerTemplate
{
	class Program
	{
		private static IHttpRequestHandler requestHandler = new DummyRequestHandler();

		static void Main(string[] args)
		{
			var listener = new TcpListener(IPAddress.Any, 80);
			listener.Start();

			while (true)
			{
				var socket = listener.AcceptSocket();
				try
				{
					using (var ns = new NetworkStream(socket) { ReadTimeout = 3000 })
					{
						var requestMessage = ReadRequestMessage(ns);

						var responseMessage = requestHandler.Handle(requestMessage);

						responseMessage.SerializeToStream(ns);

						LogSession(requestMessage, responseMessage);
						ns.Close();
					}
					socket.Disconnect(true);
				}
				catch (Exception e)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(e.Message);
					Console.ResetColor();
				}
				finally
				{
					socket.Dispose();
				}
			}
		}
		
		private static HttpRequestMessage ReadRequestMessage(Stream s)
		{
			var message = new HttpRequestMessage();
			
			string firstLine = ReadLine(s);
			var parts = firstLine.Split(' ');
			message.Method = new HttpMethod(parts[0]);
			message.Uri = new Uri(parts[1], UriKind.Relative);

			string line;
			while ((line = ReadLine(s)) != "" && line != "\r\n" && line != null)
			{
				parts = line.Split(new char[] { ':' }, 2);
				message.Headers.Add(parts[0].Trim(), parts[1].Trim());
			}
			string lengthValue;
			if (message.Headers.TryGetValue("Content-length", out lengthValue))
			{
				var length = int.Parse(lengthValue);
				var buffer = new byte[length];
				s.Read(buffer, 0, length);
				message.Body = buffer;
			}
			return message;
		}

		private static string ReadLine(Stream s)
		{
			var sb = new StringBuilder();
			char curr = ' ', prev = ' ';
			while (!(prev == '\r' && curr == '\n'))
			{
				var b = s.ReadByte();
				if(b < 0) break;
				prev = curr;
				curr = (char) b;
				sb.Append(curr);
			}
			return sb.ToString().TrimEnd('\r', '\n');
		}

		private static void LogSession(HttpRequestMessage request, HttpResponseMessage response)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(string.Format("{0} {1}", request.Method, request.Uri));
			foreach(var header in request.Headers)
			{
				Console.WriteLine(string.Format("{0}: {1}", header.Key, header.Value));
			}
			if(request.Body != null)
			{
				Console.WriteLine();
				Console.WriteLine(Encoding.ASCII.GetString(request.Body));
				Console.WriteLine();
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(string.Format("Response: {0} {1}", (int) response.StatusCode, response.StatusCode));
			Console.WriteLine();
			Console.ResetColor();
		}
	}
}
