using Newtonsoft.Json.Linq;
using qluLib.net.Enums;
using qluLib.net.Url;
using qluLib.net.Util;

namespace qluLib.net.Library;

public class Library
{
    public bool VerifyAreaSeat(Area area, SeatId seatId)
    {
        return seatId.ToString().Contains(area.ToString());
    }

    public async Task<Tuple<string[], bool>> Reserve(IUrlBase url, IEnumerable<string> cookies, AreaTime areaTime,
        Area area, SeatId seatId)
    {
        try
        {
            var enumerable = cookies.ToList();
            var userId = enumerable.FirstOrDefault(cookie => cookie.Contains("userid"))?.Split("=")[1];
            var accessToken = enumerable.FirstOrDefault(cookie => cookie.Contains("access_token"))?.Split("=")[1];
            Log.Info($"[{userId}] 开始预约");
            var libraryApi = new LibraryApi(string.Join(";", enumerable));
            var times = await libraryApi.GetTimeInfo(url);
            var day = areaTime == AreaTime.Today ? times[0] : times[1];
            Log.Info($"[{userId}] 预约时间 > {day}");
            var segment = (await libraryApi.GetAreaDays(url, area))[day];
            Log.Info($"[{userId}] Segment > {segment}");
            var postUrl = NetWorkClient.BuildUrl(
                string.Format(url.Reserve, (int)seatId),
                new SortedDictionary<string, string>
                {
                    { "access_token", accessToken },
                    { "userid", userId },
                    { "segment", segment },
                    { "type", "1" }
                }
            );
            var postHeaders = libraryApi.Headers;
            var uri = new Uri(url.Reserve);
            postHeaders.Add("Origin", $"{uri.Scheme}://{uri.Host}");
            postHeaders["Referer"] = string.Format(url.Refer, (int)area, segment, day);
            var response = await NetWorkClient.PostAsync(postUrl, null, postHeaders);
            var returnArgs = new string[] { userId, areaTime.ToString(), area.ToString(), seatId.ToString() };
            if (response is null || !response.IsSuccessStatusCode) return new Tuple<string[], bool>(returnArgs, false);
            var obj = JObject.Parse(await response.Content.ReadAsStringAsync());
            var status = obj["status"] ?? "0";
            var message = obj["msg"] ?? "";
            if (status?.ToString() == "1")
            {
                Log.Info($"[{userId}] {message.ToString().Replace("<br/>", "\n")}");
                return new Tuple<string[], bool>(returnArgs, true);
            }

            Log.Warn($" [{userId}] 预约失败\n{message}");
            return new Tuple<string[], bool>(returnArgs, false);
        }
        catch (Exception e)
        {
            Log.Error($"[ReserveException] {e.Message}");
            return new Tuple<string[], bool>([], false);
        }
    }
}