using FluentValidation;
using System.Text.RegularExpressions;

namespace SCAP.Models.Validators
{
    public class PessoaValidator<TUser> : AbstractValidator<TUser> where TUser : Pessoa
    {
        public PessoaValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("O nome é obrigatório");

            RuleFor(p => p.Sobrenome)
                .NotEmpty()
                .WithMessage("O sobrenome é obrigatório");

            RuleFor(p => p.PhoneNumber)
                .NotEmpty()
                .WithMessage("O telefone é obrigatório.")
                .Must(p => Regex.IsMatch(p, @"(\(?\d{2}\)?)\s*(\d{4,5}(\-)?\d{4})"))
                .WithMessage("Telefone inválido.");

            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
                .EmailAddress()
                .WithMessage("E-mail inválido.");

            RuleFor(p => p.UserName)
                .NotEmpty()
                .WithMessage("A matrícula é obrigatória.");
        }
    }
}
