namespace qluLib.net.Url;

public interface IUrlBase
{
    public string Sso { get; }
    public string Service { get; }
    public string FirstCookie { get; }
    public string SecondCookie { get; }
    public string TimeInfo { get; }
    public string AreaDaysSegInfo { get; }
    public string AreaReservationInfo { get; }
    public string Reserve { get; }
    public string Refer { get; }
}