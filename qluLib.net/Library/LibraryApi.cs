using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using qluLib.net.Enums;
using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Library;

public class LibraryApi(string cookie)
{
    public readonly Dictionary<string,string> Headers = new()
    {
        {"Accept", "application/json"},
        {"Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6"},
        {"Connection", "keep-alive"},
        {"Cookie", cookie},
        {"Host", "yuyue.lib.qlu.edu.cn"},
        {"Referer", "https://yuyue.lib.qlu.edu.cn/home/web/seat/area/1"},
        {"Sec-Fetch-Dest", "empty"},
        {"Sec-Fetch-Mode", "cors"},
        {"Sec-Fetch-Site", "same-origin"},
        {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0"},
        {"X-Requested-With", "XMLHttpRequest"},
        {"sec-ch-ua", "\"Chromium\";v=\"128\", \"Not;A=Brand\";v=\"24\", \"Microsoft Edge\";v=\"128\""},
        {"sec-ch-ua-mobile", "?0"},
        {"sec-ch-ua-platform", "\"Windows\""}
    };

    public async Task<List<string>> GetTimeInfo(IUrlBase url)
    {
        List<string> list = [];
        var response = await NetWorkClient.GetAsync(url.TimeInfo,Headers);
        if (response is null || !response.IsSuccessStatusCode) return [];
        var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (jObject["data"]?["list"] is not JArray jArray) return [];
        list.AddRange(jArray.Select(jToken => jToken["day"]?["date"]?.ToString().Split(" ")[0]));
        return list;
    }

    public async Task<Dictionary<string,string>> GetAreaDays(IUrlBase url,Area areaId)
    {
        Dictionary<string, string> areaDays = [];
        var response = await NetWorkClient.GetAsync(string.Format(url.AreaDaysSegInfo,(int)areaId),Headers);
        if (response is null || !response.IsSuccessStatusCode) return [];
        var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (jObject["data"]?["list"] is not JArray jArray) return [];
        foreach (var jToken in jArray)
        {
            if (jToken["day"]?.ToString() is { } day )
            {
                areaDays.Add(day,jToken["id"]?.ToString());
            }
        }
        return areaDays;
    }
    
    public async Task<Dictionary<string,SeatStatus>> GetAreaSeatStatus(IUrlBase url,Area areaId,string day)
    {
        var getUrl = NetWorkClient.BuildUrl(
            url.AreaReservationInfo, new SortedDictionary<string, string>()
            {
                { "area",areaId.ToString() },
                { "day",day },
                { "startTime","08:30" },
                { "endTime","22:00" },
            });
        var response = await NetWorkClient.GetAsync(string.Format(getUrl,(int)areaId),Headers);
        if (response is null || !response.IsSuccessStatusCode) return [];
        var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (jObject["data"]?["list"] is not JArray jArray) return [];
        var dictionary = new Dictionary<string, SeatStatus>();
        foreach (var jToken in jArray)
        {
            if (jToken["id"]?.ToString() is not { } id) continue;
            var status = jToken["status"]?.ToString() switch
            {
                "1" => SeatStatus.Free,
                "6" => SeatStatus.Using,
                "2" => SeatStatus.Reserved,
                _ => SeatStatus.Using
            };
            dictionary.Add(id,status);
        }
        return dictionary;
    }
    
}