using qluLib.net.Enums;
using qluLib.net.Library;
using qluLib.net.Sso;
using qluLib.net.Url;
using qluLib.net.Util;


//配置
var user1 = new SsoUserProfile()
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow, 
    Area = Area.二楼西, 
    SeatId = SeatId.二楼西001
};
var user2 = new SsoUserProfile()
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow, 
    Area = Area.二楼西, 
    SeatId = SeatId.二楼西002
};
var users = new List<SsoUserProfile>
{
    user1,user2
};

//预约
var sso = new SSO();
var library = new Library();
var url = new QluLibUrl();
var pastHour = -1;
var now = DateTime.Now;

Banner.Print();

while (true)
{
    switch (now.Hour)
    {
        case 21 when now is { Minute: 30, Second: 0 }:
            foreach (var user in users.Where(user => user.Verified))
            {
                user.Cookies = (await sso.GetCookies(user.Username, user.Password, url)).ToList();
            }
            break;
        case 22 when now is { Minute: 00, Second: 0 }:
        {
            var tasks = new List<Task<bool>>();
            foreach (var user in users.Where(user => user.Verified))
            {
                while (user.Cookies is null || user.Cookies.Count == 0)
                {
                    user.Cookies = (await sso.GetCookies(user.Username, user.Password, url)).ToList();
                }
                tasks.Add(library.Reserve(url, user.Cookies, user.AreaTime, user.Area, user.SeatId));
            }

            await Task.WhenAll(tasks);
            break;
        }
    }
    if(now.Hour != pastHour)
    {
        pastHour = now.Hour;
        foreach (var user in users)
        {
            if (user.Verified)
            {
                Console.WriteLine($"[{now}] [{user.Username}] [Listing..] {user.AreaTime} > {user.Area} > {user.SeatId}");
                break;
            }
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                Console.WriteLine($"[{now}] 用户名或密码为空");
                continue;
            }
            if (!library.VerifyAreaSeat(user.Area, user.SeatId))
            {
                Console.WriteLine($"[{now}] [{user.Username}] > 区域{user.Area}不存在座位{user.SeatId}");
                continue;
            }

            var cookies = await sso.GetCookies(user.Username, user.Password, url);
            if (!cookies.Any())
            {
                Console.WriteLine($"[{now}] [{user.Username}] > 账号验证失败,将在1小时后重新验证");
                continue;
            }
            Console.WriteLine($"[{now}] [{user.Username}] [Listing..] {user.AreaTime} > {user.Area} > {user.SeatId}");
            user.Verified = true;
        }
    }
    Task.Delay(1000).Wait();
}