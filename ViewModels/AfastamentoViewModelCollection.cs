using System.Collections.Generic;

namespace SCAP.ViewModels
{
    public class AfastamentoViewModelCollection
    {
        public IEnumerable<AfastamentoViewModel> Afastamentos { get; set; }
        public IEnumerable<AfastamentoViewModel> AfastamentosAsSolicitante { get; set; }
        public IEnumerable<AfastamentoViewModel> AfastamentosAsRelator { get; set; }
    }
}
