using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Models
{
    public class Professor : Pessoa
    {
        /* EF Associations */
        [InverseProperty("ParenteDe")]
        public IEnumerable<Parentesco> Parentescos { get; set; }

        [InverseProperty("Parente")]
        public IEnumerable<Parentesco> ParentescoDe { get; set; }

        public IEnumerable<Mandato> Mandatos { get; set; }
        public IEnumerable<Parecer> Pareceres { get; set; }

        [InverseProperty("Relator")]
        public IEnumerable<Afastamento> AfastamentosComoRelator { get; set; }

        [InverseProperty("Solicitante")]
        public IEnumerable<Afastamento> Afastamentos { get; set; }
    }
}
