using System;
using System.ComponentModel.DataAnnotations;

namespace SCAP.Models
{
    public class Parecer : Entity
    {
        [Required]
        public DateTime DataEmissao { get; set; }

        [Required]
        public TipoParecer Julgamento { get; set; }

        [StringLength(1024)]
        public string Justificativa { get; set; }
        
        /* EF Associations */
        [Required]
        public string ProfessorId { get; set; }
        public Professor Professor { get; set; }

        [Required]
        public Guid AfastamentoId { get; set; }
        public Afastamento Afastamento { get; set; }
    }
}
