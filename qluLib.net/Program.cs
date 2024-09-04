using qluLib.net.Enums;
using qluLib.net.Lib;
using qluLib.net.Sso;
using qluLib.net.Url;

namespace qluLib.net;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var sso = new SSO();
        
        var library = new Library();
        var url = new QluLibUrl();
        
        const string userName = "你的学号";
        const string passWord = "你的密码";
        
        var cookies = await sso.GetCookies(userName,passWord,url);
        await library.Reserve(url, cookies, AreaTime.Tomorrow, Area.六楼东, SeatId.六楼东001);
    }
    
    
}