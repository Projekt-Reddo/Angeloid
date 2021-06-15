using System;
using System.Net.Mail;
using System.Threading.Tasks;

//Models
using Angeloid.Models;

namespace Angeloid.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        private readonly string _frontEndUrl;
        public EmailService(EmailConfig emailConfig, string frontEndUrl) {
            this._emailConfig = emailConfig;
            this._frontEndUrl = frontEndUrl;
        }

        public async Task SendEmailAsync(string email, string guid) {
            try {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(this._emailConfig.MailAddress);
                mail.To.Add(email);
                mail.Subject = "[Angeloid] Reset Password";
                mail.IsBodyHtml = true;
                
                mail.Body = $"Click the button to reset your password<br><br><a href=\"{this._frontEndUrl + "/account/change/" + guid.ToString()}\"><button style=\"background-color: green;border: none; border-radius: 8rem;height: 40px; width: 120px;color: white;\">Reset Password</button></a><br><br>This link will expired after 30 minutes!";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(this._emailConfig.MailAddress, this._emailConfig.MailPassword);
                SmtpServer.EnableSsl = true;

                await SmtpServer.SendMailAsync(mail);
            } catch (Exception e) {
                System.Console.WriteLine(e);
            }
        }
    }
}