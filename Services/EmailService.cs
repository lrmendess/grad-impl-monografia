using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using SCAP.Models;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SCAP.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserService<Pessoa> _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public EmailService(IEmailSender emailSender, IUserService<Pessoa> userService,
            IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _emailSender = emailSender;
            _userService = userService;

            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

        public async Task SendEmailDefinePassword(Pessoa user)
        {
            var token = _userService.GeneratePasswordResetToken(user);

            string encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Email));
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
                action: "DefinePassword",
                controller: "Users",
                values: new { encodedEmail, encodedToken },
                scheme: _httpContextAccessor.HttpContext.Request.Scheme);

            var subject = "Defina uma senha para sua conta";
            var message =
                $"Olá," +
                $"<br/>" +
                $"<br/>" +
                $"<a href='{ HtmlEncoder.Default.Encode(callbackUrl) }'>Clique aqui</a> para definir uma senha para a sua conta." +
                $"<br/>" +
                $"<br/>" +
                $"Favor não responder este email.";

            await _emailSender.SendEmailAsync(user.Email, subject, message);
        }

        public async Task SendEmailAboutAfastamento(Afastamento afastamento, Pessoa pessoa, string subject, string message = "", string link = "")
        {
            await SendEmailAboutAfastamento(afastamento, new List<Pessoa> { pessoa }, subject, message, link);
        }
        
        public async Task SendEmailAboutAfastamento(Afastamento afastamento, IEnumerable<Pessoa> pessoas, string subject, string message = "", string link = "")
        {
            string _link;

            if (_httpContextAccessor.HttpContext == null)
            {
                if (String.IsNullOrEmpty(link))
                {
                    return;
                }
                else
                {
                    _link = link;
                }
            }
            else
            {
                if (String.IsNullOrEmpty(link))
                {
                    _link = _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext,
                        action: "Details",
                        controller: "Afastamentos",
                        values: new { id = afastamento.Id },
                        scheme: _httpContextAccessor.HttpContext.Request.Scheme);
                }
                else
                {
                    _link = link;
                }
            }

            var emails = string.Join(",", pessoas.Select(p => p.Email));
            var extra = String.IsNullOrEmpty(message) ? "" : $"<p>{message}</p>";
            var body =
                $"Olá," +
                $"<br/>" +
                $"<br/>" +
                extra +
                $"<a href='{ HtmlEncoder.Default.Encode(_link) }'>Clique aqui</a> para ver os detalhes do pedido de afastamento." +
                $"<br/>" +
                $"<br/>" +
                $"Caso o link acima não esteja funcionando, entre no SCAP e busque pelo pedido {afastamento.Id}." +
                $"<br/>" +
                $"<br/>" +
                $"Favor não responder este email.";

            await _emailSender.SendEmailAsync(emails, subject, body);
        }
    }
}
