using System.Net;
using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Sso;

public class SsoLoginData
{
    public string Crypto { get; set; }
    public string Execution { get; set; }
    public List<Cookie> Cookies { get; set; }

    public override string ToString()
    {
        return $"Crypto: {Crypto}, Execution: {Execution}";
    }
}

public class SsoApi
{
    public async Task<SsoLoginData> GetSsoLoginData(IUrlBase urlBase)
    {
        var cookieContainer = new CookieContainer();
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(3),
            CookieContainer = cookieContainer
        };
        var httpClient = new HttpClient(socketsHttpHandler);
        var uri = new Uri(urlBase.Sso);
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to get SsoLoginData");
        }
        var cookies = cookieContainer.GetCookies(uri).ToList();
        var pageString = await response.Content.ReadAsStringAsync();
        var croyptoMath = Regexes.CroyptoRegex().Match(pageString);
        var executionMatch = Regexes.ExecutionRegex().Match(pageString);
        
        
        return new SsoLoginData
        {
            Cookies = cookies,
            Crypto = croyptoMath.Groups[1].Value,
            Execution = executionMatch.Groups[1].Value,
        };

    }

    public async Task<HttpResponseMessage> Login(IUrlBase url,string username, string password,SsoLoginData loginData)
    {
        var encryptedPassword = Crypto.DesEncrypt(loginData.Crypto, password);
        //Console.WriteLine($"Encrypted Password: {encryptedPassword}");
        
        var headers = new Dictionary<string, string>
        {
            { "cookie",$"SESSION={loginData.Cookies[0].Value}" },
        };
        var data = new Dictionary<string, string>
        {
            { "username", username },
            { "croypto", loginData.Crypto },
            { "password", encryptedPassword },
            { "type", "UsernamePassword" },
            { "_eventId","submit" },
            { "geolocation","" }, 
            { "execution",loginData.Execution }, 
            { "captcha_code","" },
        };
        var httpContent = new FormUrlEncodedContent(data);
        
        return await NetWorkClient.PostAsync(url.Sso,httpContent,headers);
    }

    public async Task<IEnumerable<string>> GetCookies(IUrlBase url)
    {
        var response = await NetWorkClient.GetAsync(url.FirstCookie);
        if (!response.IsSuccessStatusCode) return [];
        var cookies = response.Headers.GetValues("Set-Cookie")
            .Select(cookie => cookie.Split(';')[0])
            .ToList();
        cookies.AddRange(await NetWorkClient.GetCookiesAsString(url.SecondCookie));
        return cookies;
    }

    public async Task<string> GetCookiesAsString(IUrlBase url) => string.Join(";", await GetCookies(url));
    
    
}