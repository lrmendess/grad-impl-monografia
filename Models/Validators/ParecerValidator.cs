using FluentValidation;

namespace SCAP.Models.Validators
{
    public class ParecerValidator : AbstractValidator<Parecer>
    {
        public ParecerValidator()
        {
            RuleFor(p => p.DataEmissao)
                .NotEmpty()
                .WithMessage("A data de emissão é obrigatória");

            RuleFor(p => p.Julgamento)
                .NotEmpty()
                .WithMessage("O julgamento é obrigatória.")
                .IsInEnum()
                .WithMessage("Julgamento inválio.");

            // Quando o parecer for desfavorável, é necessário se justificar.
            When(p => p.Julgamento == TipoParecer.Desfavoravel, () =>
            {
                RuleFor(p => p.Justificativa)
                    .NotEmpty()
                    .WithMessage("A justificativa é obrigatória para pareceres desfavoráveis.")
                    .Length(2, 1024)
                    .WithMessage("A justificativa deve ter no mínimo {MinLength} e no máximo {MaxLength} caracteres.");
            });

            RuleFor(p => p.ProfessorId)
                .NotEmpty()
                .WithMessage("O professor responsável pelo parecer é obrigatório.");

            RuleFor(p => p.AfastamentoId)
                .NotEmpty()
                .WithMessage("O afastamento a qual o parecer se refere é obrigatório.");
        }
    }
}
