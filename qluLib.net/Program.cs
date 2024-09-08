using qluLib.net.Enums;
using qluLib.net.Library;
using qluLib.net.Sso;
using qluLib.net.Url;
using qluLib.net.Util;

//配置
//启用邮箱通知
const bool mailNotify = true;
//如启用邮箱通知,需补全以下信息
var mailData = new MailData
{
    //smtp 邮箱账号
    FromAddress = "", //example@qq.com
    //smtp 生成密码
    Password = "",
    //邮件接收地址
    To = ["example@qq.com", "example1@qq.com"], //example@qq.com
    //启用ssl 默认启用
    EnableSsl = true,
    //smtp服务器
    Host = "", //example smtp.qq.com
    //smtp服务器端口
    Port = 0 //example 587
};

var user1 = new SsoUserProfile
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow,
    Area = Area.二楼西,
    SeatId = SeatId.二楼西001
};
var user2 = new SsoUserProfile
{
    Username = "",
    Password = "",
    AreaTime = AreaTime.Tomorrow,
    Area = Area.二楼西,
    SeatId = SeatId.二楼西002
};
var users = new List<SsoUserProfile>
{
    user1, user2
};

//预约
var sso = new Sso();
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
            foreach (var user in users.Where(user => user.Verified))
                user.Cookies = (await sso.GetCookies(user.Username, user.Password, url)).ToList();
            break;
        case 22 when now is { Minute: 00, Second: 0 }:
        {
            var tasks = new List<Task<Tuple<string[], bool>>>();
            foreach (var user in users.Where(user => user.Verified))
            {
                while (user.Cookies is null || user.Cookies.Count == 0)
                    user.Cookies = (await sso.GetCookies(user.Username, user.Password, url)).ToList();
                tasks.Add(library.Reserve(url, user.Cookies, user.AreaTime, user.Area, user.SeatId));
            }

            await Task.WhenAll(tasks);
            if (mailNotify)
                foreach (var task in tasks.Where(task => task.IsCompletedSuccessfully))
                {
                    mailData.Subject = $"{task.Result.Item1[0]} {task.Result.Item2}";
                    mailData.Body = task.Result.Item2
                        ? $"{string.Join(" > ", task.Result.Item1)} 预约成功"
                        : $"[{string.Join(" > ", task.Result.Item1)}] 预约失败";
                    MailClient.SendEmailAnonymous(mailData);
                }

            break;
        }
    }

    if (now.Hour != pastHour)
    {
        pastHour = now.Hour;
        if (!mailData.Verified)
        {
            mailData.Subject = $"qluLib.net < Device:{Environment.MachineName}";
            mailData.Body = "预约通知服务开启";
            if (!MailClient.VerifyMailData(mailData))
                Log.Warn("预约通知服务验证失败,将在1小时后重试");
            else
                mailData.Verified = true;
        }

        foreach (var user in users)
        {
            if (!user.Verified)
            {
                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    Log.Warn("用户名或密码为空");
                    continue;
                }

                if (!library.VerifyAreaSeat(user.Area, user.SeatId))
                {
                    Log.Warn($"[{user.Username}] > 区域{user.Area}不存在座位{user.SeatId}");
                    continue;
                }

                var cookies = await sso.GetCookies(user.Username, user.Password, url);
                if (!cookies.Any())
                {
                    Log.Warn($"[{user.Username}] > 账号验证失败,将在1小时后重试");
                    continue;
                }

                user.Verified = true;
            }

            Log.Info($"[{user.Username}] [Listening..] {user.AreaTime} > {user.Area} > {user.SeatId}");
        }
    }

    Task.Delay(1000).Wait();
}