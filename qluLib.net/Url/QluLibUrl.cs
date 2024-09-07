using qluLib.net.Util;

namespace qluLib.net.Url;

public class QluLibUrl : IUrlBase
{
    public string Sso => "https://sso.qlu.edu.cn/login";

    public string Service =>
        "https://yuyue.lib.qlu.edu.cn/cas/index.php?callback=https://yuyue.lib.qlu.edu.cn/home/web/seat/area/1";

    public string FirstCookie =>
        NetWorkClient.BuildUrl(Sso, new SortedDictionary<string, string> { { "service", Service } });

    public string SecondCookie => "https://yuyue.lib.qlu.edu.cn/home/web/seat/area/1";
    public string TimeInfo => "https://yuyue.lib.qlu.edu.cn/api.php/areadays/1";
    public string AreaDaysSegInfo => "https://yuyue.lib.qlu.edu.cn/api.php/areadays/{0}";
    public string AreaReservationInfo => "https://yuyue.lib.qlu.edu.cn/api.php/spaces_old";
    public string Reserve => "https://yuyue.lib.qlu.edu.cn/api.php/spaces/{0}/book";

    public string Refer =>
        "https://yuyue.lib.qlu.edu.cn/web/seat3?area={0}&segment={1}&day={2}&startTime=08:30&endTime=22:00";
}