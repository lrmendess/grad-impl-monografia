using FluentValidation;
using KissLog;
using SCAP.Data.Interfaces;
using SCAP.Models;
using SCAP.Models.Validators;
using SCAP.Notifications;
using SCAP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCAP.Services
{
    public class MandatoService : EntityService<Mandato>, IMandatoService
    {
        public MandatoService(IUnitOfWork unitOfWork, INotificator notificator, ILogger logger)
            : base(unitOfWork, notificator, logger) { }

        public override Mandato Add(Mandato mandato)
        {
            #region Validation
            var validator = new MandatoValidator();

            validator.RuleFor(m => m.ProfessorId)
                .Must(pid => UnitOfWork.Professores.Exists(pid))
                .WithMessage("Professor não encontrado.");

            var vigentesIni = UnitOfWork.Mandatos.GetMandatosVigentes(mandato.DataInicio);
            var vigentesFim = UnitOfWork.Mandatos.GetMandatosVigentes(mandato.DataFim);
            var vigentes = vigentesIni.Concat(vigentesFim);

            validator.RuleFor(m => m.TipoMandato)
                .Must(tipo => !vigentes.Any(v => v.TipoMandato == tipo))
                .WithMessage("Já existe um mandato de (sub)chefe cadastrado dentro do intervalo estipulado.");

            validator.RuleFor(m => m.ProfessorId)
                .Must(pid => !vigentes.Any(v => v.ProfessorId == pid))
                .WithMessage("Este professor já possui um mandato em vigor");

            if (!IsValid(validator, mandato))
                return null;
            #endregion

            return base.Add(mandato);
        }

        public IEnumerable<Mandato> GetMandatosVigentes() =>
            UnitOfWork.Mandatos.GetMandatosVigentes(DateTime.Now);

        public IEnumerable<Mandato> GetAllWithProfessor() =>
            UnitOfWork.Mandatos.GetAllWithProfessor();

        public Mandato GetWithProfessor(Guid id) =>
            UnitOfWork.Mandatos.GetWithProfessor(id);

        public void Interromper(Guid id)
        {
            #region Validation
            var validator = new MandatoValidator();

            var now = DateTime.Now.Date;

            validator.RuleFor(m => m)
                .Must(m => now <= m.DataFim && !m.Interrompido)
                .WithMessage("Este mandato não está em vigor para que possa ser interrompido.");

            var mandato = UnitOfWork.Mandatos.Get(id);

            if (!IsValid(validator, mandato))
                return;
            #endregion

            mandato.DataFim = now;
            mandato.Interrompido = true;

            base.Update(mandato);
        }

        public override void Remove(Guid id)
        {
            #region Validation
            var validator = new MandatoValidator();
            
            var now = DateTime.Now.Date;

            validator.RuleFor(m => m.DataInicio)
                .Must(di => now < di)
                .WithMessage("Não é possível excluir um mandato que já tenha começado.");

            if (!IsValid(validator, UnitOfWork.Mandatos.Get(id)))
                return;
            #endregion

            base.Remove(id);
        }
    }
}
