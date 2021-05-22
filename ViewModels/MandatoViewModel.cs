using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SCAP.ViewModels
{
    public class MandatoViewModel
    {
        [Key]
        [HiddenInput]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Data de início do mandato")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Data de fim do mandato")]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Tipo de mandato")]
        [Range(1, 2, ErrorMessage = "O campo {0} precisa ser igual a Chefe ou Subchefe")]
        public int TipoMandato { get; set; }

        [DefaultValue(false)]
        public bool Interrompido { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Professor")]
        public string ProfessorId { get; set; }
        public UserViewModel Professor { get; set; }
    }
}