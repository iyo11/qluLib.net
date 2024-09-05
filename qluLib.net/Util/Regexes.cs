using System.Text.RegularExpressions;

namespace qluLib.net.Util;

public static partial class Regexes
{
    [GeneratedRegex(@"<p id=""login-croypto"">(.+?)<\/p>", RegexOptions.Compiled)]
    public static partial Regex CroyptoRegex();
    [GeneratedRegex(@"<p id=""login-page-flowkey"">([^<]+)</p>", RegexOptions.Compiled)]
    public static partial Regex ExecutionRegex();
    [GeneratedRegex(@"[\\u4e00-\\u9fff]+", RegexOptions.Compiled)]
    public static partial Regex ChineseCharactersRegex();
}