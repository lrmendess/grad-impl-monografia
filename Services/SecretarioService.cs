using KissLog;
using Microsoft.AspNetCore.Identity;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;

namespace SCAP.Services
{
    public class SecretarioService : UserService<Secretario>, ISecretarioService
    {
        public SecretarioService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger,
            UserManager<Pessoa> userManager) : base(unitOfWork, notificator, logger, userManager) { }
    }
}
