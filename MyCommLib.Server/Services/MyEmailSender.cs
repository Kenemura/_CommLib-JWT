using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MyCommLib.Server.Data;

namespace MyCommLib.Server.Services;
public class MyEmailSender
{
    private readonly ConfigData dc;
    private List<SendEmailSmtp> SmtpInfos = [];
    private readonly ILogger logger;

    public MyEmailSender(ConfigData dc, ILogger<MyEmailSender> logger)
    {
        this.dc = dc;
        this.logger = logger;
        AddSmtpInfos();
    }
    private string FromName { get; set; } = "ICS WebApp";
    private string Subject { get; set; } = "";
    private string HtmlBody { get; set; } = "";
    private string TextBody { get; set; } = "";

    private readonly List<MailboxAddress> ToList = [];
    private readonly List<MailboxAddress> CcList = [];
    private readonly List<string> Attachments = [];

    public void AddAttachment(string path) { Attachments.Add(path); }
    public void AddFromName(string name) { FromName = name; }
    public void AddTo(string name, string email) { ToList.Add(new(name, email)); }
    public void AddCc(string name, string email) { CcList.Add(new(name, email)); }
    public void AddSubject(string subject) { Subject = subject; }
    public void AddHtmlBody(string body) { HtmlBody = body; }
    public void AddTextBody(string body) { TextBody = body; }

    public async Task<string> Send()
    {
        await Task.CompletedTask;
        var result = "";
        foreach (var smtp in SmtpInfos)
        {
            result = await Send(smtp);
            if (String.IsNullOrEmpty(result)) return "";
        }
        return result;
    }
    private async Task<string> Send(SendEmailSmtp smtp)
    {
        logger.LogInformation($"Sending via: {smtp.Host}");
        SmtpClient client = new();
        try
        {
            MimeMessage msg = new();
            MailboxAddress from = new(FromName, smtp.User);
            msg.From.Add(from);
            foreach (var to in ToList) { msg.To.Add(to); }
            foreach (var cc in CcList) { msg.Cc.Add(cc); }
            msg.Subject = Subject;
            BodyBuilder bb = new BodyBuilder();
            bb.HtmlBody = HtmlBody;
            bb.TextBody = TextBody;
            foreach (string path in Attachments)
            {
                bb.Attachments.Add(path);
            }
            msg.Body = bb.ToMessageBody();

            client.Connect(smtp.Host, smtp.Port, MailKit.Security.SecureSocketOptions.Auto);
            client.Authenticate(smtp.User, smtp.Password);

            await client.SendAsync(msg);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return ex.Message;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
        return "";
    }
    private void AddSmtpInfos()
    {
        for (int i = 1; i < 10; i++)
        {
            AddSmtpInfo(i.ToString());
        }
    }
    private void AddSmtpInfo(string name)
    {
        var host = dc.ConfigKVPs.FirstOrDefault(x => x.Key == $"smtp:{name}:Host")?.Value ?? "";
        if (String.IsNullOrEmpty(host)) return;
        var port = dc.ConfigKVPs.FirstOrDefault(x => x.Key == $"smtp:{name}:Port")?.Value ?? "";
        var user = dc.ConfigKVPs.FirstOrDefault(x => x.Key == $"smtp:{name}:User")?.Value ?? "";
        var pwd = dc.ConfigKVPs.FirstOrDefault(x => x.Key == $"smtp:{name}:Password")?.Value ?? "";
        var iPort = 0;
        Int32.TryParse(port, out iPort);
        SmtpInfos.Add(new SendEmailSmtp() { Host = host, Port = iPort, User = user, Password = pwd });
    }

    internal class SendEmailSmtp
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
