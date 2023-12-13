namespace RaceRoute.Web;

public class ProxySpaDevServer
{
    private readonly RequestDelegate next;
    private readonly string baseUrl;

    public ProxySpaDevServer(RequestDelegate next, string baseUrl)
    {
        this.next = next;
        this.baseUrl = baseUrl;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path;
        if (!Path.HasExtension(path) && context.GetEndpoint() == null && !path.Value.StartsWith("/@")) 
        {
            context.Request.Path = path = "/index.html";
        }

        if (Path.HasExtension(path) || path.Value.StartsWith("/@")) 
        {
            await ProxyRequest(context);
        } else {
            await next(context);
        }
    }

    private HttpRequestMessage GetProxyRequest(HttpContext context) 
    {
        var baseUri = new Uri(baseUrl);
        var targetUri = new Uri(baseUri, context.Request.Path + context.Request.QueryString);

        var request = context.Request;
        var requestMessage = new HttpRequestMessage();

        // Copy the request headers
        foreach (var header in request.Headers)
        {
            if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }

        requestMessage.Headers.Host = targetUri.Authority;
        requestMessage.RequestUri = targetUri;
        requestMessage.Method = new HttpMethod(request.Method);

        return requestMessage;
    }

    private async Task ProxyRequest(HttpContext context)
    {
        using var httpClient =  new HttpClient();
        using var requestMessage = GetProxyRequest(context);
        using var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

        context.Response.StatusCode = (int)responseMessage.StatusCode;
        foreach (var header in responseMessage.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in responseMessage.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
        context.Response.Headers.Remove("transfer-encoding");

        var sss = await responseMessage.Content.ReadAsStringAsync();
        using var responseStream = await responseMessage.Content.ReadAsStreamAsync();

        await responseStream.CopyToAsync(context.Response.Body, 81920);
    }
}