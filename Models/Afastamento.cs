using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Models
{
    public class Afastamento : Entity
    {
        [Required]
        public DateTime DataPedido { get; set; }

        [Required]
        public DateTime DataInicio { get; set; }

        [Required]
        public DateTime DataFim { get; set; }

        [Required]
        public SituacaoPedidoAfastamento Situacao { get; set; }

        [Required]
        public TipoAfastamento TipoAfastamento { get; set; }

        [StringLength(1024)]
        public string Motivo { get; set; }

        [Required]
        public Onus Onus { get; set; }

        [Required]
        public DateTime DataInicioEvento { get; set; }

        [Required]
        public DateTime DataFimEvento { get; set; }

        [Required]
        [StringLength(256)]
        public string NomeEvento { get; set; }

        /* EF Associations */
        public IEnumerable<Parecer> Pareceres { get; set; }
        public IEnumerable<Documento> Documentos { get; set; }

        [ForeignKey("RelatorId")]
        public Professor Relator { get; set; }
        public string RelatorId { get; set; }

        [ForeignKey("SolicitanteId")]
        public Professor Solicitante { get; set; }
        [Required]
        public string SolicitanteId { get; set; }
    }
}
