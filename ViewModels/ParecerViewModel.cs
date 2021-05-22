using System;
using System.ComponentModel.DataAnnotations;

namespace SCAP.ViewModels
{
    public class ParecerViewModel
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime DataEmissao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, 2, ErrorMessage = "O {0} deve ser favorável ou desfavorável.")]
        public int Julgamento { get; set; }
        
        [StringLength(1024)]
        public string Justificativa { get; set; }

        public string ProfessorId { get; set; }
        public UserViewModel Professor { get; set; }

        [Required]
        public Guid AfastamentoId { get; set; }
        public AfastamentoViewModel Afastamento { get; set; }

        public DocumentoViewModel DocumentoParecer { get; set; }
    }
}