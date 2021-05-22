using System;
using System.ComponentModel.DataAnnotations;

namespace SCAP.Models
{
    public class Documento : Entity
    {
        [Required]
        [StringLength(128)]
        public string Titulo { get; set; }
        
        [Required]
        public string NomeArquivo { get; set; }

        [Required]
        public DateTime DataSubmissao { get; set; }

        /* EF Associations */
        [Required]
        public Guid AfastamentoId { get; set; }
        public Afastamento Afastamento { get; set; }
    }
}
