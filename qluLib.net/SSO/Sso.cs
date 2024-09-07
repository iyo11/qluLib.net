using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Sso;

public class Sso
{
    public async Task<IEnumerable<string>> GetCookies(string ssoUserName, string ssoPassword, IUrlBase url)
    {
        try
        {
            NetWorkClient.InitHttpClient();
            var ssoApi = new SsoApi();
            var loginData = await ssoApi.GetSsoLoginData(url,ssoUserName);
            Log.Info($"[{ssoUserName}] Successfully obtained SsoLoginData");
            var response = await ssoApi.Login(url, ssoUserName, ssoPassword, loginData);
            Log.Info($"[{ssoUserName}] Unified pass login successful");
            if (response is null || !response.IsSuccessStatusCode)
            {
                return [];
            }
            Log.Info($"[{ssoUserName}] Obtaining Cookies");
            var cookies = (await ssoApi.GetCookies(url,ssoUserName)).ToList();
            Log.Info($"[{ssoUserName}] Successfully obtained Cookies");
            if (cookies.Count != 0)
            {
                Log.Info($"[{ssoUserName}] Cookies={string.Join(";",cookies)}");
            }
            return cookies;
        }
        catch (Exception e)
        {
            Log.Error($"[GetCookiesException] {e.Message}");
            return [];
        }
    }
}