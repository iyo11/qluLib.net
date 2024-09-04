using qluLib.net.Url;

namespace qluLib.net.Sso;

public class SSO
{
    public async Task<IEnumerable<string>> GetCookies(string ssoUserName, string ssoPassword, IUrlBase url)
    {
        try
        {
            var ssoApi = new SsoApi();
            var loginData = await ssoApi.GetSsoLoginData(url);
            await ssoApi.Login(url, ssoUserName, ssoPassword, loginData);
            var cookies = (await ssoApi.GetCookies(url)).ToList();
            Console.WriteLine($"[{ssoUserName}] cookies -> {string.Join(";",cookies)}");
            return cookies;
        }
        catch (Exception e)
        {
            Console.WriteLine("Unable to get cookies");
            Console.WriteLine(e.Message);
            throw;
        }
    }
}