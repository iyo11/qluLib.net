namespace qluLib.net.Util;

public static class Banner
{
    public static void Print()
    {
        const string banner = @" 
 ________  ___       ___  ___  ___       ___  ________      ________   _______  _________   
|\   __  \|\  \     |\  \|\  \|\  \     |\  \|\   __  \    |\   ___  \|\  ___ \|\___   ___\ 
\ \  \|\  \ \  \    \ \  \\\  \ \  \    \ \  \ \  \|\ /_   \ \  \\ \  \ \   __/\|___ \  \_| 
 \ \  \\\  \ \  \    \ \  \\\  \ \  \    \ \  \ \   __  \   \ \  \\ \  \ \  \_|/__  \ \  \  
  \ \  \\\  \ \  \____\ \  \\\  \ \  \____\ \  \ \  \|\  \ __\ \  \\ \  \ \  \_|\ \  \ \  \ 
   \ \_____  \ \_______\ \_______\ \_______\ \__\ \_______\\__\ \__\\ \__\ \_______\  \ \__\
    \|___| \__\|_______|\|_______|\|_______|\|__|\|_______\|__|\|__| \|__|\|_______|   \|__|
          \|__|                                                                             ";
        Console.WriteLine(banner);
    }
}