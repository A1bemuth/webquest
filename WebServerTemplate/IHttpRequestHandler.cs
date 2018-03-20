namespace WebServerTemplate
{
	public interface IHttpRequestHandler
	{
		HttpResponseMessage Handle(HttpRequestMessage request);
	}
}
