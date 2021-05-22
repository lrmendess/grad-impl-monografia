using FluentValidation;

namespace SCAP.Models.Validators
{
    public class MandatoValidator : AbstractValidator<Mandato>
    {
        public MandatoValidator()
        {
            RuleFor(m => m.DataInicio.Date)
                .NotEmpty()
                .WithMessage("A data de início é obrigatória.");

            RuleFor(m => m.DataFim.Date)
                .NotEmpty()
                .WithMessage("A data de fim é obrigatória.")
                .GreaterThan(m => m.DataInicio.Date)
                .WithMessage("A data de fim do mandato deve ser maior do que a data de início do mesmo");

            RuleFor(m => m.ProfessorId)
                .NotEmpty()
                .WithMessage("O professor escolhido para ser chefe de departamento é obrigatório.");
        }
    }
}
