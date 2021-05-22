using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCAP.Models
{
    public class Parentesco : Entity
    {
        [Required]
        public TipoParentesco TipoParentesco { get; set; }

        /* EF Associations */
        [ForeignKey("ParenteDeId")]
        public Professor ParenteDe { get; set; }
        [Required]
        public string ParenteDeId { get; set; }

        [ForeignKey("ParenteId")]
        public Professor Parente { get; set; }
        [Required]
        public string ParenteId { get; set; }
    }
}
