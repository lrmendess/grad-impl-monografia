using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SCAP.ViewModels
{
    public class UserViewModel
    {
        [Key]
        [HiddenInput]
        public string Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, 2, ErrorMessage = "Escolha um opção válida para o campo {0}")]
        [DisplayName("Tipo de usuário")]
        public int UserType { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter no máximo {1} dígitos.")]
        [RegularExpression("([0-9]+)", ErrorMessage = "O campo {0} deve conter apenas números")]
        [DisplayName("Número de matrícula")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(128, ErrorMessage = "A {0} deve ter no máximo {1} caracteres.")]
        [DisplayName("Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(128, ErrorMessage = "A {0} deve ter no máximo {1} caracteres.")]
        [DisplayName("Sobrenome")]
        public string Sobrenome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [EmailAddress(ErrorMessage = "O email digitado é inválido")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Phone]
        [DisplayName("Telefone")]
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [RegularExpression(@"(\(?\d{2}\)?)\s*(\d{4,5}(\-)?\d{4})", ErrorMessage = "Telefone inválido")]
        public string PhoneNumber { get; set; }

        [DisplayName("Situação")]
        [DefaultValue(true)]
        public bool Ativo { get; set; }

        public IEnumerable<ParentescoViewModel> Parentescos { get; set; }
        public IEnumerable<ParentescoViewModel> ParentescoDe { get; set; }
        public IEnumerable<MandatoViewModel> Mandatos { get; set; }
        public IEnumerable<ParecerViewModel> Pareceres { get; set; }
        public IEnumerable<AfastamentoViewModel> AfastamentosComoRelator { get; set; }
        public IEnumerable<AfastamentoViewModel> Afastamentos { get; set; }
    }
}
