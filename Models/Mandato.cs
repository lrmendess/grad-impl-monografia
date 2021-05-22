using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Models
{
    public class Mandato : Entity
    {
        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public TipoMandato TipoMandato { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Interrompido { get; set; }

        /* EF Associations */
        [Required]
        public string ProfessorId { get; set; }
        public Professor Professor { get; set; }
    }
}
