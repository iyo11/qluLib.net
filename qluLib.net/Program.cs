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
var pastHour = 0;

while (true)
{
    DateTime now = DateTime.Now;
    if (now.Hour == 21 && now.Minute == 30 && now.Second == 0)
    {
        cookies = (await sso.GetCookies(userName, passWord, url)).ToList();
    }
    if (now.Hour == 22 && now.Minute == 00 && now.Second == 0){
        if (cookies.Count == 0)
        {
            cookies = (await sso.GetCookies(userName, passWord, url)).ToList();
        }
        await library.Reserve(url, cookies, areaTime, area, seatId);
        cookies.Clear();
    }
    if(now.Hour != pastHour)
    {
        pastHour = now.Hour;
        Console.WriteLine($"[{now}] {userName} -> {areaTime} > {area} > {seatId}");
    }
    Task.Delay(500).Wait();
}