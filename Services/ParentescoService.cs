using FluentValidation;
using KissLog;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Linq;

namespace SCAP.Services
{
    public class ParentescoService : EntityService<Parentesco>, IParentescoService
    {
        public ParentescoService(IUnitOfWork unitOfWork, ILogger logger, INotificator notificator)
            : base(unitOfWork, notificator, logger) { }

        public override Parentesco Add(Parentesco entity)
        {
            #region Validation
            var validator = new ParentescoValidator();

            validator.RuleFor(p => new { p.ParenteId, p.ParenteDeId })
                .Must(newp => !UnitOfWork.Parentescos.Search(p =>
                    (p.ParenteId == newp.ParenteDeId && p.ParenteDeId == newp.ParenteId) ||
                    (p.ParenteId == newp.ParenteId && p.ParenteDeId == newp.ParenteDeId)).Any())
                .WithMessage("Este parentesco já está cadastrado.");

            if (!IsValid(validator, entity))
                return null;
            #endregion

            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                var result = UnitOfWork.Parentescos.Add(entity);
                UnitOfWork.Parentescos.Add(new Parentesco
                {
                    ParenteDeId = entity.ParenteId,
                    ParenteId = entity.ParenteDeId,
                    TipoParentesco = entity.TipoParentesco
                });

                UnitOfWork.SaveChanges();
                transaction.Commit();

                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");

                return null;
            }
        }

        public override void Remove(Guid id)
        {
            var parentesco = UnitOfWork.Parentescos.Get(id);

            #region Validation
            if (!IsValid(new ParentescoValidator(), parentesco))
                return;
            #endregion

            using var transaction = UnitOfWork.BeginTransaction();

            try
            {
                var reverseParentesco = UnitOfWork.Parentescos.Search(p => p.ParenteId == parentesco.ParenteDeId && p.ParenteDeId == parentesco.ParenteId);

                UnitOfWork.Parentescos.Remove(parentesco.Id);
                UnitOfWork.Parentescos.Remove(reverseParentesco.First().Id);

                UnitOfWork.SaveChanges();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Logger.Error(e.Message);
                Error(string.Empty, "Ocorreu um erro inesperado.");
            }
        }
    }
}
