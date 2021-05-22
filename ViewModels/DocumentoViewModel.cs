using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SCAP.ViewModels
{
    public class DocumentoViewModel
    {
        [Key]
        [HiddenInput]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "O campo {0} deve ter no mínimo {2} e no máximo {1} caracteres.")]
        [DisplayName("Nome do documento")]
        public string Titulo { get; set; }

        public string NomeArquivo { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DataSubmissao { get; set; }

        [Required(ErrorMessage = "Selecione um arquivo.")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [HiddenInput]
        [DisplayName("Afastamento associado")]
        public Guid AfastamentoId { get; set; }
        public AfastamentoViewModel Afastamento { get; set; }
    }
}