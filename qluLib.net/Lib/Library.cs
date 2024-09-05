using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using qluLib.net.Enums;
using qluLib.net.Sso;
using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Lib;

public class Library
{
    public bool VerifyAreaSeat(Area area,SeatId seatId) => seatId.ToString().Contains(area.ToString());
    
    public async Task<bool> Reserve(IUrlBase url, IEnumerable<string> cookies, AreaTime areaTime, Area area, SeatId seatId)
    {
        try
        {
            var enumerable = cookies.ToList();
            var userId = enumerable.FirstOrDefault(cookie => cookie.Contains($"userid"))?.Split("=")[1];
            var accessToken = enumerable.FirstOrDefault(cookie => cookie.Contains($"access_token"))?.Split("=")[1];
            Console.WriteLine($"{userId} 开始预约");
            var libraryApi = new LibraryApi(string.Join(";", enumerable));
            var times = await libraryApi.GetTimeInfo(url);
            var day = areaTime == AreaTime.Today ? times[0] : times[1];
            Console.WriteLine($"[{userId}] 预约时间 -> {day}");
            var segment = (await libraryApi.GetAreaDays(url, area))[day];
            Console.WriteLine($"[{userId}] Segment -> {segment}");
            var postUrl = NetWorkClient.BuildUrl(
                string.Format(url.Reserve, (int)seatId),
                new SortedDictionary<string, string>()
                {
                    { "access_token", accessToken },
                    { "userid", userId },
                    { "segment", segment },
                    { "type", "1" },
                }
            );
            var postHeaders = libraryApi.Headers;
            var uri = new Uri(url.Reserve);
            postHeaders.Add("Origin", $"{uri.Scheme}://{uri.Host}");
            postHeaders["Referer"] = string.Format(url.Refer, (int)area, segment, day);
            var responseMessage = await NetWorkClient.PostAsync(postUrl, null, postHeaders);
            if (!responseMessage.IsSuccessStatusCode) return false;
            var obj = JObject.Parse(await responseMessage.Content.ReadAsStringAsync());
            var status = obj["status"] ?? "0";
            var message = obj["msg"] ?? "";
            if (status?.ToString() == "1")
            {
                Console.WriteLine($"[{userId}] {message.ToString().Replace("<br/>","\n")}");
                return true;
            }

            Console.WriteLine($"[{userId}] 预约失败\n{message}");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Unable to reserve");
            Console.WriteLine(e.Message);
            throw;
        }
    }
}