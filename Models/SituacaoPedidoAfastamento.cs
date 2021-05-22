using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Models
{
    public enum SituacaoPedidoAfastamento
    {
        Iniciado = 1,
        Liberado = 2,
        Bloqueado = 3,
        AprovadoDI = 4,
        AprovadoCT = 5,
        AprovadoPRPPG = 6,
        Cancelado = 7,
        Reprovado = 8,
        Arquivado = 9
    }
}
