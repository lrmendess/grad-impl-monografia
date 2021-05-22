using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SCAP.ViewModels
{
    public class AfastamentoViewModel
    {
        [Key]
        [HiddenInput]
        [DisplayName("Código do pedido")]
        public Guid Id { get; set; }

        [DisplayName("Data e hora do pedido")]
        [ScaffoldColumn(false)]
        public DateTime DataPedido { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Data de início")]
        public DateTime DataInicio { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Data de fim")]
        public DateTime DataFim { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, 9, ErrorMessage = "O campo {0} deve conter uma opção válida.")]
        [DisplayName("Situação atual")]
        public int Situacao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, 2, ErrorMessage = "O campo {0} deve conter uma opção válida.")]
        [DisplayName("Tipo de afastamento")]
        public int TipoAfastamento { get; set; }

        [StringLength(1024, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        [DisplayName("Motivo do afastamento")]
        public string Motivo { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Range(1, 3 , ErrorMessage = "O campo {0} deve conter uma opção válida.")]
        [DisplayName("Tipo de ônus")]
        public int Onus { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Data de início do evento")]
        public DateTime DataInicioEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [DisplayName("Data de fim do evento")]
        public DateTime DataFimEvento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "O campo {0} deve ter no mínimo {2} e no máximo {1} caracteres.")]
        [DisplayName("Nome do evento")]
        public string NomeEvento { get; set; }
        
        public List<ParecerViewModel> Pareceres { get; set; }
        public List<DocumentoViewModel> Documentos { get; set; }
        
        [HiddenInput]
        [DisplayName("Relator")]
        public string RelatorId { get; set; }
        public UserViewModel Relator { get; set; }

        [HiddenInput]
        [DisplayName("Solicitante")]
        public string SolicitanteId { get; set; }
        public UserViewModel Solicitante { get; set; }

        [DisplayName("Decisão")]
        [Range(1, 2, ErrorMessage = "O campo {0} deve conter uma opção válida.")]
        public int DecisaoReuniao { get; set; }
    }
}