using FluentValidation;

namespace SCAP.Models.Validators
{
    public class ParentescoValidator : AbstractValidator<Parentesco>
    {
        public ParentescoValidator()
        {
            RuleFor(p => p.TipoParentesco)
                .NotEmpty()
                .WithMessage("O tipo de parentesco é obrigatório.")
                .IsInEnum()
                .WithMessage("Tipo de parentesco inválido.");

            RuleFor(p => p.ParenteDeId)
                .NotEmpty()
                .WithMessage("O parente de origem é obrigatório");

            RuleFor(p => p.ParenteId)
                .NotEmpty()
                .WithMessage("O parente de destino é obrigatório.")
                .NotEqual(p => p.ParenteDeId)
                .WithMessage("Parente de origem e destino não podem ser a mesma pessoa.");
        }
    }
}
