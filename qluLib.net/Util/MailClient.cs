using System.Net;
using System.Net.Mail;

namespace qluLib.net.Util;

public class MailData
{
    public List<string> To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string FromAddress { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; } = false;
    public bool IsBodyHtml { get; set; } = false;
    public bool Verified { get; set; } = false;
}

public static class MailClient
{
    public static bool VerifyMailData(MailData data)
    {
        try
        {
            SendEmailAnonymous(data);
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"Failed to verify mailData {e.Message}");
            return false;
        }
    }

    public static void SendEmailAnonymous(MailData data)
    {
        using var mail = new MailMessage();
        foreach (var to in data.To)
        {
            mail.To.Add(to);
        }
        mail.From = new MailAddress(data.FromAddress);
        mail.Subject = data.Subject; 
        mail.Body = data.Body; 
        mail.IsBodyHtml = data.IsBodyHtml;
        // 配置SMTP服务器
        using var smtp = new SmtpClient(data.Host);
        smtp.Port = data.Port;
        smtp.Credentials = new NetworkCredential(data.FromAddress, data.Password);
        smtp.EnableSsl = true;
        smtp.EnableSsl = data.EnableSsl; 
        smtp.Port = data.Port;
        
        try
        {
            Log.Info($"Trying to send email [{data.Subject}]");
            smtp.Send(mail);
            Log.Info($"Successfully sent email [{data.Subject}] ");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to send email: {ex}");
        }
    }
}