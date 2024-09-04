using qluLib.net.Enums;
using qluLib.net.Lib;
using qluLib.net.Sso;
using qluLib.net.Url;
//你的学号
const string userName = "";
//你的sso密码
const string passWord = "";
const Area area = Area.六楼东;
const AreaTime areaTime = AreaTime.Tomorrow;
const SeatId seatId = SeatId.六楼东001;

var sso = new SSO();
var library = new Library();
var url = new QluLibUrl();
List<string> cookies =[];
var pastHour = -1;

while (true)
{
    var now = DateTime.Now;
    switch (now.Hour)
    {
        case 21 when now is { Minute: 30, Second: 0 }:
            cookies = (await sso.GetCookies(userName, passWord, url)).ToList();
            break;
        case 22 when now is { Minute: 00, Second: 0 }:
        {
            if (cookies.Count == 0)
            {
                cookies = (await sso.GetCookies(userName, passWord, url)).ToList();
            }
            await library.Reserve(url, cookies, areaTime, area, seatId);
            cookies.Clear();
            break;
        }
    }

    if(now.Hour != pastHour)
    {
        pastHour = now.Hour;
        Console.WriteLine($"[{now}] {userName} -> {areaTime} > {area} > {seatId}");
    }
    Task.Delay(500).Wait();
}