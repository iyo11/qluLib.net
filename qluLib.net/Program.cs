using qluLib.net.Enums;
using qluLib.net.Lib;
using qluLib.net.Sso;
using qluLib.net.Url;
using qluLib.net.Util;

//配置
var user1 = new SsoUserProfile()
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow, 
    Area = Area.六楼东, 
    SeatId = SeatId.六楼东001
};
var user2 = new SsoUserProfile()
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow, 
    Area = Area.六楼东, 
    SeatId = SeatId.六楼东002
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
Banner.Print();
while (true)
{
    var now = DateTime.Now;
    switch (now.Hour)
    {
        case 21 when now is { Minute: 30, Second: 0 }:
            foreach (var user in users)
            {
                user.Cookies = (await sso.GetCookies(user.Username, user.Password, url)).ToList();
            }
            break;
        case 22 when now is { Minute: 00, Second: 0 }:
        {
            var tasks = new List<Task<bool>>();
            foreach (var user in users)
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
        foreach (var user in users)
        {
            pastHour = now.Hour;
            Console.WriteLine($"[{now}] {user.Username} -> {user.AreaTime} > {user.Area} > {user.SeatId}"); 
        }
    }
    Task.Delay(1000).Wait();
}