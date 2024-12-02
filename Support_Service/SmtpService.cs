using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using Ecommerce_Product.Models;
using MimeKit.Cryptography;
using System.Security.Policy;
namespace Ecommerce_Product.Support_Serive;
public class SmtpService
{
private readonly SmtpModel _smtpClient;

private readonly Service _spService;

private readonly ILogger<SmtpService> _logger;


public SmtpService(IOptions<SmtpModel> smtpClient,Service spService,ILogger<SmtpService> logger)
{
    this._smtpClient=smtpClient.Value;
    this._spService=spService;
    this._logger=logger;
}


public string htmlContent(string receiver,string operating_system,string random_password,int role=1)
{
    string htmlContent="";
    
    string path=this._spService.GetCurrentFilePath("Views/MailTemplate/SendMailTemplate.html");
    
    using(StreamReader sr=new StreamReader(path))
    {
        htmlContent=sr.ReadToEnd();
    }
 string url="";
if(role==1)
{
 url="https://thanhquang-gnss.com/LoginAdmin/ChangePassword?email="+receiver+"&password="+random_password;
}
else{
 url="https://thanhquang-gnss.com/MyAccount/ChangePassword?email="+receiver+"&password="+random_password;

}

    htmlContent=htmlContent.Replace("{name}",receiver);
    htmlContent=htmlContent.Replace("{operating_system}",operating_system);
    //htmlContent=htmlContent.Replace("{browser_name}",browser_name);
    htmlContent=htmlContent.Replace("{new_password}",random_password);
    htmlContent=htmlContent.Replace("{email_value}",receiver);
    htmlContent=htmlContent.Replace("_cur_url_",url);
    Console.WriteLine(url);

    return htmlContent;
}


// public string getCurrentBrowser()
// {
//     string web_brower=HttpContext..Headers["User-Agent"].ToString();
// }
public async Task sendEmail(string new_password,string receiver,string subject,int role=1)
{    
  try{
    // Console.WriteLine("Port:"+this._smtpClient.Port);
    // Console.WriteLine("Username:"+this._smtpClient.Username);
    //     Console.WriteLine("Password:"+this._smtpClient.Password);

    // Console.WriteLine("SenderName:"+this._smtpClient.SenderName);

    // Console.WriteLine("SenderEmail:"+this._smtpClient.SenderEmail);

    // Console.WriteLine("Usessl:"+this._smtpClient.UseSsl);

    // Console.WriteLine("Host:"+this._smtpClient.Host);

    string currentOs=this._spService.getCurrentOs();
    string htmlValue=htmlContent(receiver,currentOs,new_password,role);
    Console.WriteLine(currentOs);
   var emailMessage = new MimeMessage();

   emailMessage.From.Add(new MailboxAddress(this._smtpClient.SenderName,this._smtpClient.SenderEmail));

   emailMessage.To.Add(new MailboxAddress("",receiver));

   emailMessage.Subject=subject;

   var bodyBuilder =new BodyBuilder{HtmlBody=htmlValue};
   
   emailMessage.Body=bodyBuilder.ToMessageBody();
   
   using(var client = new SmtpClient())
   {
      await client.ConnectAsync(_smtpClient.Host,this._smtpClient.Port,this._smtpClient.UseSsl);
      Console.WriteLine("did here");
      await client.AuthenticateAsync(this._smtpClient.Username,this._smtpClient.Password);
      Console.WriteLine("Did come to here");
      await client.SendAsync(emailMessage);
      await client.DisconnectAsync(true);
   }
  }
  catch(Exception er)
  { this._logger.LogTrace("Smtp Exception:"+er.Message);
    Console.WriteLine("Send Smtp Exception:"+er.Message);
  }
}
}