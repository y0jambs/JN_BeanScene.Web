using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BeanScene.Web.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Read SMTP settings from appsettings.json
            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"]);
            var enableSsl = bool.Parse(_config["SmtpSettings:UseSsl"]);
            var user = _config["SmtpSettings:UserName"];
            var password = _config["SmtpSettings:Password"];
            var fromEmail = _config["SmtpSettings:FromEmail"];
            var fromName = _config["SmtpSettings:FromName"];

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, password)
            };

            using var message = new MailMessage()
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            message.To.Add(email);
            await client.SendMailAsync(message);
        }
    }
}