using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace qluLib.net.Util;

public static class NetWorkClient
{
    private static HttpClient _httpClient = CreateHttpClient();
    private static CookieContainer _cookieContainer = new();
    private static HttpClient CreateHttpClient()
    {
        var socketsHttpHandler = new SocketsHttpHandler() 
        {
            ConnectTimeout = TimeSpan.FromSeconds(3),
            CookieContainer = _cookieContainer
        };
        return new HttpClient(socketsHttpHandler);
    }

    public static void InitHttpClient()
    {
        _cookieContainer = new CookieContainer();
        _httpClient = CreateHttpClient();
    }
    
    public static async Task<IEnumerable<Cookie>> GetCookies(string url)
    {
        var cookieContainer = new CookieContainer();
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(3),
            CookieContainer = cookieContainer
        };
        var httpClient = new HttpClient(socketsHttpHandler);
        var uri = new Uri(url);
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to get cookies");
            return [];
        }
        var cookies = cookieContainer.GetCookies(uri).ToList();
        return cookies;
    }

    public static async Task<List<string>> GetCookiesAsString(string url) => (await GetCookies(url)).Select(cookie => cookie.ToString()).ToList();
    
    
    public static async ValueTask<HttpResponseMessage> GetAsync(string url,Dictionary<string, string> headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        if (headers is not null)
        {
            foreach (var keyValuePair in headers)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}.");
        }
        return response;
    }
    
    public static async ValueTask<HttpResponseMessage> PostAsync(
        string url, 
        HttpContent content, 
        Dictionary<string, string> headers = null)
    {
       var request = new HttpRequestMessage(HttpMethod.Post, url);
       request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
       if (headers is not null)
       {
           foreach (var keyValuePair in headers)
           {
               request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
           }
       }
       if (content is not null)
       {
           request.Content = content;
       }
       var response = await _httpClient.SendAsync(request);
       if (!response.IsSuccessStatusCode)
       {
           throw new HttpRequestException($"Request failed with status code {response.StatusCode}.");
       }
       return response;
    }
    
    public static string BuildUrl(string url, SortedDictionary<string, string> dic)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(url);
        stringBuilder.Append('?');
        stringBuilder.Append(BuildParam(dic));
        return stringBuilder.ToString();
    }

    private static string BuildParam(SortedDictionary<string, string> dic)
    {
        var stringBuilder = new StringBuilder();
        foreach (var item in dic)
        {
            stringBuilder
                .Append(item.Key)
                .Append('=')
                .Append(WebUtility.UrlEncode(item.Value))
                .Append('&');
        }
        
        return stringBuilder.ToString();

    }
}