using SCAP.Models;
using System;
using System.Collections.Generic;

namespace SCAP.Services
{
    public static class AfastamentoStateMachine
    {
        private static readonly Dictionary<SituacaoPedidoAfastamento, List<SituacaoPedidoAfastamento>> _nacional = new Dictionary<SituacaoPedidoAfastamento, List<SituacaoPedidoAfastamento>>
        {
            [SituacaoPedidoAfastamento.Iniciado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.Liberado, SituacaoPedidoAfastamento.Cancelado },
            [SituacaoPedidoAfastamento.Liberado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoDI, SituacaoPedidoAfastamento.Bloqueado, SituacaoPedidoAfastamento.Cancelado },
            [SituacaoPedidoAfastamento.Bloqueado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoDI, SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Reprovado },
            [SituacaoPedidoAfastamento.AprovadoDI] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Arquivado },
            [SituacaoPedidoAfastamento.Cancelado] = new List<SituacaoPedidoAfastamento> { },
            [SituacaoPedidoAfastamento.Arquivado] = new List<SituacaoPedidoAfastamento> { },
            [SituacaoPedidoAfastamento.Reprovado] = new List<SituacaoPedidoAfastamento> { }
        };

        private static readonly Dictionary<Enum, List<SituacaoPedidoAfastamento>> _internacional = new Dictionary<Enum, List<SituacaoPedidoAfastamento>>
        {
            [SituacaoPedidoAfastamento.Iniciado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.Liberado, SituacaoPedidoAfastamento.Cancelado },
            [SituacaoPedidoAfastamento.Liberado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoDI, SituacaoPedidoAfastamento.Bloqueado, SituacaoPedidoAfastamento.Cancelado },
            [SituacaoPedidoAfastamento.Bloqueado] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoDI, SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Reprovado },
            [SituacaoPedidoAfastamento.AprovadoDI] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoCT, SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Reprovado },
            [SituacaoPedidoAfastamento.AprovadoCT] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.AprovadoPRPPG, SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Reprovado },
            [SituacaoPedidoAfastamento.AprovadoPRPPG] = new List<SituacaoPedidoAfastamento> { SituacaoPedidoAfastamento.Cancelado, SituacaoPedidoAfastamento.Arquivado },
            [SituacaoPedidoAfastamento.Cancelado] = new List<SituacaoPedidoAfastamento> { },
            [SituacaoPedidoAfastamento.Arquivado] = new List<SituacaoPedidoAfastamento> { },
            [SituacaoPedidoAfastamento.Reprovado] = new List<SituacaoPedidoAfastamento> { }
        };

        public static bool CanChangeTo(this Afastamento afastamento, SituacaoPedidoAfastamento to)
        {
            if (afastamento.TipoAfastamento == TipoAfastamento.Nacional)
            {
                if (_nacional.ContainsKey(afastamento.Situacao))
                {
                    return _nacional[afastamento.Situacao].Contains(to);
                }
            }
            else if (afastamento.TipoAfastamento == TipoAfastamento.Internacional)
            {
                if (_internacional.ContainsKey(afastamento.Situacao))
                {
                    return _internacional[afastamento.Situacao].Contains(to);
                }
            }

            return false;
        }
    }
}
