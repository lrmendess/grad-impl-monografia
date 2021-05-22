using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SCAP.ViewModels
{
    public class ParentescoViewModel
    {
        [Key]
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Tipo de parentesco")]
        [Range(1, 2, ErrorMessage = "O {0} deve ser selecionado.")]
        public int TipoParentesco { get; set; }

        [Required]
        [HiddenInput]
        public string ParenteDeId { get; set; }
        public UserViewModel ParenteDe { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Parente")]
        [HiddenInput]
        public string ParenteId { get; set; }
        public UserViewModel Parente { get; set; }
    }
}
