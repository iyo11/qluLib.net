using qluLib.net.Enums;

namespace qluLib.net.Sso;

public class SsoUserProfile
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Area Area { get; set; }
    public AreaTime AreaTime { get; set; }
    public SeatId SeatId { get; set; }
    public List<string> Cookies { get; set; }
    public bool Verified { get; set; } = false;
}

