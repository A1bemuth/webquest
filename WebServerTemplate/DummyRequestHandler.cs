using System.Text;
using System.Net;

namespace WebServerTemplate
{
	public class DummyRequestHandler : IHttpRequestHandler
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
}
