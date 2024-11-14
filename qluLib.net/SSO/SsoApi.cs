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
    public async Task<SsoLoginData> GetSsoLoginData(IUrlBase urlBase, string username)
    {
        var cookieContainer = new CookieContainer();
        var socketsHttpHandler = new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(30),
            CookieContainer = cookieContainer
        };
        var httpClient = new HttpClient(socketsHttpHandler);
        var uri = new Uri(urlBase.Sso);
        Log.Info($"[{username}] Obtaining SessionId");
        var response = await httpClient.GetAsync(uri);
        if (!response.IsSuccessStatusCode) Log.Warn($"[{username}] Failed to get SsoLoginData");
        var cookies = cookieContainer.GetCookies(uri).ToList();
        Log.Info($"[{username}] Successfully obtained SessionId");
        var pageString = await response.Content.ReadAsStringAsync();
        var croyptoMath = Regexes.CroyptoRegex().Match(pageString);
        var executionMatch = Regexes.ExecutionRegex().Match(pageString);

        return new SsoLoginData
        {
            Cookies = cookies,
            Crypto = croyptoMath.Groups[1].Value,
            Execution = executionMatch.Groups[1].Value
        };
    }

    public async Task<HttpResponseMessage> Login(IUrlBase url, string username, string password, SsoLoginData loginData)
    {
        var encryptedPassword = Crypto.AesEncrypt(loginData.Crypto, password);
        //Console.WriteLine($"Encrypted Password: {encryptedPassword}");

        var headers = new Dictionary<string, string>
        {
            { "cookie", $"SESSION={loginData.Cookies[0].Value}" }
        };
        var data = new Dictionary<string, string>
        {
            { "username", username },
            { "croypto", loginData.Crypto },
            { "password", encryptedPassword },
            { "type", "UsernamePassword" },
            { "_eventId", "submit" },
            { "geolocation", "" },
            { "execution", loginData.Execution },
            { "captcha_code", "" }
        };
        var httpContent = new FormUrlEncodedContent(data);
        return await NetWorkClient.PostAsync(url.Sso, httpContent, headers);
    }

    public async Task<IEnumerable<string>> GetCookies(IUrlBase url, string username)
    {
        Log.Info($"[{username}] Obtaining cookies part[1]");
        var response = await NetWorkClient.GetAsync(url.FirstCookie);
        if (response is null || !response.IsSuccessStatusCode) return [];
        Log.Info($"[{username}] Successfully obtained cookies part[1]");
        var cookies = response.Headers.GetValues("Set-Cookie")
            .Select(cookie => cookie.Split(';')[0])
            .ToList();
        Log.Info($"[{username}] Obtaining cookies part[2]");
        cookies.AddRange(await NetWorkClient.GetCookiesAsString(url.SecondCookie));
        Log.Info($"[{username}] [SsoApi] Successfully obtained cookies part[2]");
        return cookies;
    }

    public async Task<string> GetCookiesAsString(IUrlBase url, string username)
    {
        return string.Join(";", await GetCookies(url, username));
    }
}