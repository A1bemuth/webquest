using System.Text;
using System.Net;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebServerTemplate
{
	public class QuestRequestHandler : IHttpRequestHandler
	{
		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var handler = GetSpecificHandler(request);
			return handler.Handle(request);
		}

		private IHttpRequestHandler GetSpecificHandler(HttpRequestMessage request)
		{
			switch (request.Method.ToString().ToUpper())
			{
				case "GET":
					return SelectGetHandler(request);
				case "POST":
					return SelectPostHandler(request);
				default:
					return new NotFoundHandler();
			}
		}

		private IHttpRequestHandler SelectGetHandler(HttpRequestMessage request)
		{
			var path = request.Uri.ToString().Split('?')[0];
			if(path.Equals("/helloworld"))
				return new HelloWorldHandler();
			if(path.Equals("/sum"))
				return new SumHandler();
			if(path.Equals("/storage"))
				return new StorageHandler();

			return new NotFoundHandler();
		}

		private IHttpRequestHandler SelectPostHandler(HttpRequestMessage request)
		{
			var path = request.Uri.ToString().Split('?')[0];
			if (path.Equals("/sumFromBody"))
				return new SumFromBodyHandler();
			if (path.Equals("/storage"))
				return new StorageHandler();

			return new NotFoundHandler();
		}
	}

	public class StorageHandler : IHttpRequestHandler
	{
		private static object storage;

		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var responseMessage = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
			};

			if(request.Method == HttpMethod.Get)
			{
				responseMessage.Body = storage as byte[] ?? Encoding.ASCII.GetBytes("Oops");
			}
			else
			{
				storage = request.Body;
				responseMessage.Body = Encoding.ASCII.GetBytes("Stored safe and sound");
			}
			
			responseMessage.Headers.Add("Content-length", responseMessage.Body.Length.ToString());
			responseMessage.Headers.Add("Content-type", "text/html");
			return responseMessage;
		}
	}

	public class SumFromBodyHandler : IHttpRequestHandler
	{
		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var content = JObject.Parse(Encoding.ASCII.GetString(request.Body));
			var numbers = content["numbers"].ToObject<int[]>();

			var response = JsonConvert.SerializeObject(new {result = numbers.Sum()});
			var responseMessage = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Body = Encoding.ASCII.GetBytes(response)
			};
			responseMessage.Headers.Add("Content-length", responseMessage.Body.Length.ToString());
			responseMessage.Headers.Add("Content-type", "text/html");
			return responseMessage;
		}
	}

	public class SumHandler : IHttpRequestHandler
	{
		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var query = request.Uri.ToString().Split('?')[1]
				.Split('&')
				.Select(s => s.Split('='))
				.ToDictionary(p => p[0], p => int.Parse(p[1]));
			var responseMessage = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Body = Encoding.ASCII.GetBytes((query["a"]+query["b"]).ToString())
			};
			responseMessage.Headers.Add("Content-length", responseMessage.Body.Length.ToString());
			responseMessage.Headers.Add("Content-type", "text/plain");
			return responseMessage;
		}
	}

	public class HelloWorldHandler : IHttpRequestHandler
	{
		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var responseMessage = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Body = Encoding.ASCII.GetBytes("Hello, world!")
			};
			responseMessage.Headers.Add("Content-length", responseMessage.Body.Length.ToString());
			responseMessage.Headers.Add("Content-type", "text/html");
			return responseMessage;
		}
	}

	public class NotFoundHandler : IHttpRequestHandler
	{
		public HttpResponseMessage Handle(HttpRequestMessage request)
		{
			var responseMessage = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.NotFound,
				Body = Encoding.ASCII.GetBytes("This is not the uri u're searching for")
			};
			responseMessage.Headers.Add("Content-length", responseMessage.Body.Length.ToString());
			responseMessage.Headers.Add("Content-type", "text/html");
			return responseMessage;
		}
	}
}
