using SCAP.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCAP.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailDefinePassword(Pessoa user);
        Task SendEmailAboutAfastamento(Afastamento afastamento, Pessoa pessoa, string subject, string message = "", string link = "");
        Task SendEmailAboutAfastamento(Afastamento afastamento, IEnumerable<Pessoa> pessoas, string subject, string message = "", string link = "");
    }
}
