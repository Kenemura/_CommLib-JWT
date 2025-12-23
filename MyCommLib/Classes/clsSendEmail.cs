namespace MyCommLib.Classes;

using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class clsSendEmail
{
    public string FromName { get; set; } = "ICS WebApp";
    public string Subject { get; set; } = "";
    public string HtmlBody { get; set; } = "";
    public string TextBody { get; set; } = "";

    private List<MailboxAddress> ToList = new List<MailboxAddress>();
    private List<MailboxAddress> CcList = new List<MailboxAddress>();
    private List<string> Attachments = new List<string>();
    public clsSendEmail() {}
    public void AddAttachment(string path) { Attachments.Add(path); }
    public void AddTo(string name, string email) { ToList.Add(new MailboxAddress(name, email)); }
    public void AddCc(string name, string email) { CcList.Add(new MailboxAddress(name, email)); }
    public async Task<string> Send() // Use default local SmtpInfos
    {
        await Task.CompletedTask;
        return await Send(SmtpInfos);
    }
    public async Task<string> Send(List<clsSendEmailSmtp> smtpInfos) // Use Smtp Infos in the parameter
    {
        await Task.CompletedTask;
        var result = "";
        foreach (var smtp in smtpInfos)
        {
            result = await Send(smtp);
            if (String.IsNullOrEmpty(result)) return "";
        }
        return result;
    }
    public async Task<string> Send(clsSendEmailSmtp smtp)
    {
        SmtpClient client = new SmtpClient();
        try
        {
            MimeMessage msg = new MimeMessage();
            MailboxAddress from = new MailboxAddress(FromName, smtp.User);
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
            return ex.Message;
        }
        finally
        {
            client.Disconnect(true);
            client.Dispose();
        }
        return "";
    }

    public void AddSmtpInfo(string host, int port, string user, string pwd)
    {
        SmtpInfos.Add(new clsSendEmailSmtp() { Host = host, Port = port, User = user, Password = pwd }); 
    }
    private List<clsSendEmailSmtp> SmtpInfos = new List<clsSendEmailSmtp>();
}
public class clsSendEmailSmtp
{
    public string Name { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string User { get; set; } = null!;
    public string Password { get; set; } = null!;
}
