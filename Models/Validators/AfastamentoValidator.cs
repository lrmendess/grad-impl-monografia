using FluentValidation;
using System;

namespace SCAP.Models.Validators
{
    public class AfastamentoValidator : AbstractValidator<Afastamento>
    {
        public AfastamentoValidator()
        {
            RuleFor(a => a.DataPedido)
                .NotEmpty()
                .WithMessage("A data do pedido do afastamento é obrigatória.");

            RuleFor(a => a.DataInicio.Date)
                .NotEmpty()
                .WithMessage("A data de início do afastamento é obrigatória.")
                .GreaterThanOrEqualTo(a => a.DataPedido.Date)
                .WithMessage("A data de início do afastamento deve ser maior ou igual a data do pedido do mesmo (hoje).");

            RuleFor(a => a.DataFim.Date)
                .NotEmpty()
                .WithMessage("A data de fim do afastamento é obrigatória.")
                .GreaterThanOrEqualTo(a => a.DataInicio.Date)
                .WithMessage("A data de fim do afastamento deve ser maior ou igual a data de início do mesmo.");

            RuleFor(a => a.Situacao)
                .NotEmpty()
                .WithMessage("A situação do afastamento é obrigatória.")
                .IsInEnum()
                .WithMessage("Situação inválida.");
            
            RuleFor(a => a.TipoAfastamento)
                .NotEmpty()
                .WithMessage("O tipo de afastamento é obrigatório.")
                .IsInEnum()
                .WithMessage("Tipo de afastamento inválido.");

            RuleFor(a => a.Motivo)
                .Length(0, 1024)
                .WithMessage("O motivo deve ter no máximo {MaxLength} caracteres.");

            RuleFor(a => a.Onus)
                .NotEmpty()
                .WithMessage("O ônus é obrigatório.")
                .IsInEnum()
                .WithMessage("Ônus inválido");
            
            RuleFor(a => a.DataInicioEvento.Date)
                .NotEmpty()
                .WithMessage("A data de início do evento é obrigatória.");

            RuleFor(a => a.DataFimEvento.Date)
                .NotEmpty()
                .WithMessage("A data de fim do evento é obrigatória.")
                .GreaterThanOrEqualTo(a => a.DataInicioEvento.Date)
                .WithMessage("A data de fim do evento deve ser maior ou igual a data de início do mesmo.");

            RuleFor(a => a.NomeEvento)
                .NotEmpty()
                .WithMessage("O nome do evento é obrigatório.")
                .Length(2, 256)
                .WithMessage("O nome do evento deve ter no mínimo {MinLength} e no máximo {MaxLength} caracteres.");
        
            RuleFor(a => a.SolicitanteId)
                .NotEmpty()
                .WithMessage("O solicitante é obrigatório.");

            /**
             * Valida se o solicitante e relator não são a mesma pessoa.
             */
            RuleFor(a => a.RelatorId)
                .NotEqual(a => a.SolicitanteId)
                .When(a => !String.IsNullOrEmpty(a.RelatorId))
                .WithMessage("Solicitante e relator não podem ser a mesma pessoa.");

            /**
             * Valida se há interseção entre o range das datas do afastamento e do evento.
             * 
             * Ref: https://stackoverflow.com/questions/325933/determine-whether-two-date-ranges-overlap
             */
            RuleFor(a => new { a.DataInicio, a.DataFim, a.DataInicioEvento, a.DataFimEvento })
                .Must(d => (d.DataInicio <= d.DataFimEvento) && (d.DataFim >= d.DataInicioEvento))
                .WithMessage("Deve haver uma interseção entre os períodos de afastamento e evento.");
        }
    }
}
