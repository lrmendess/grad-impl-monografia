using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SCAP.Configurations
{
    public static class EmailSenderConfig
    {
        public static IServiceCollection ResolveEmailSender(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailSender, EmailSender>(i =>
                new EmailSender
                (
                    configuration["EmailSender:Host"],
                    Convert.ToInt32(configuration["EmailSender:Port"]),
                    Convert.ToBoolean(configuration["EmailSender:EnableSSL"]),
                    configuration["EmailSender:FromEmail"],
                    configuration["EmailSender:UserName"],
                    configuration["EmailSender:Password"]
                )
            );

            return services;
        }
    }

    public class EmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSSL;
        private readonly string _fromEmail;
        private readonly string _userName;
        private readonly string _password;

        public EmailSender(string host, int port, bool enableSSL, string fromEmail, string userName, string password)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSL;
            _fromEmail = fromEmail;
            _userName = userName;
            _password = password;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_host, _port)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSSL
            };

            await client.SendMailAsync(
                new MailMessage(_fromEmail, email, subject, htmlMessage)
                {
                    IsBodyHtml = true
                }
            );
        }
    }
}
