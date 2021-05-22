using FluentValidation;

namespace SCAP.Models.Validators
{
    public class DocumentoValidator : AbstractValidator<Documento>
    {
        public DocumentoValidator()
        {
            RuleFor(d => d.Titulo)
                .NotEmpty()
                .WithMessage("O título é obrigatório.")
                .Length(2, 128)
                .WithMessage("O título deve ter no mínimo {MinLength} e no máximo {MaxLength} caracteres.");

            RuleFor(d => d.NomeArquivo)
                .NotEmpty()
                .WithMessage("O arquivo é obrigatório.");

            RuleFor(d => d.DataSubmissao)
                .NotEmpty()
                .WithMessage("A data de submissão é obrigatória");

            RuleFor(d => d.AfastamentoId)
                .NotEmpty()
                .WithMessage("O afastamento a qual o documento pertence deve ser informado.");
        }
    }
}
