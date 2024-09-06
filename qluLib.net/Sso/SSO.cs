using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Sso;

public class SSO
{
    public async Task<IEnumerable<string>> GetCookies(string ssoUserName, string ssoPassword, IUrlBase url)
    {
        try
        {
            NetWorkClient.InitHttpClient();
            var ssoApi = new SsoApi();
            var loginData = await ssoApi.GetSsoLoginData(url,ssoUserName);
            Console.WriteLine($"[{DateTime.Now}] [{ssoUserName}] [SSO] Successfully obtained SsoLoginData");
            var response = await ssoApi.Login(url, ssoUserName, ssoPassword, loginData);
            Console.WriteLine($"[{DateTime.Now}] [{ssoUserName}] [SSO] Unified pass login successful");
            if (response is null || !response.IsSuccessStatusCode)
            {
                return [];
            }
            Console.WriteLine($"[{DateTime.Now}] [{ssoUserName}] [SSO] Obtaining Cookies");
            var cookies = (await ssoApi.GetCookies(url,ssoUserName)).ToList();
            Console.WriteLine($"[{DateTime.Now}] [{ssoUserName}] [SSO] Successfully obtained Cookies");
            if (cookies.Count != 0)
            {
                Console.WriteLine($"[{TimeDate.Now}] [{ssoUserName}] [SSO] Cookies={string.Join(";",cookies)}");
            }
            return cookies;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[{TimeDate.Now}] [SSO] [GetCookiesException] {e.Message}");
            return [];
        }
    }
}