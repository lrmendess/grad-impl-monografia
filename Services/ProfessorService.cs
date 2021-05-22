using FluentValidation;
using KissLog;
using Microsoft.AspNetCore.Identity;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System.Linq;

namespace SCAP.Services
{
    public class ProfessorService : UserService<Professor>, IProfessorService
    {
        public ProfessorService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger,
            UserManager<Pessoa> userManager) : base(unitOfWork, notificator, logger, userManager) { }

        public Professor GetChefe() =>
            UnitOfWork.Professores.GetChefe();

        public bool IsChefe(Professor professor) =>
            UnitOfWork.Professores.IsChefe(professor);

        public Professor GetWithParentescos(string id) =>
            UnitOfWork.Professores.GetWithParentescos(id);

        public override void Remove(string id)
        {
            #region Validation
            var validator = new PessoaValidator<Professor>();

            validator.RuleFor(p => p)
                .Must(p => !p.Mandatos.Any() && !p.Parentescos.Any() && !p.ParentescoDe.Any() && !p.Afastamentos.Any() && !p.AfastamentosComoRelator.Any() && !p.Pareceres.Any())
                .WithMessage("Não é possível excluir um professor com mandatos, parentescos, afastamentos ou pareceres.");

            var professor = UnitOfWork.Professores.GetWithAll(id);

            if (!IsValid(validator, professor))
                return;
            #endregion

            base.Remove(id);
        }
    }
}
