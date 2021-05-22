using System.Collections.Generic;

namespace SCAP.ViewModels
{
    public class MandatoViewModelCollection
    {
        public IEnumerable<MandatoViewModel> Chefes { get; set; }
        public IEnumerable<MandatoViewModel> Agendados { get; set; }
        public IEnumerable<MandatoViewModel> Arquivados { get; set; }
    }
}
