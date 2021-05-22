using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class AfastamentoRepository : EntityRepository<Afastamento>, IAfastamentoRepository
    {
        public AfastamentoRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IEnumerable<Afastamento> GetAllWithSolicitanteAndRelator()
        {
            return _context.Afastamentos
                .Include(a => a.Solicitante)
                .Include(a => a.Relator)
                .AsNoTracking()
                .ToList();
        }

        public Afastamento GetWithAll(Guid id)
        {
            return _context.Afastamentos
                .Include(a => a.Solicitante)
                .Include(a => a.Relator)
                .Include(a => a.Pareceres)
                    .ThenInclude(p => p.Professor)
                .Include(a => a.Documentos)
                .FirstOrDefault(a => a.Id == id);
        }

        public Afastamento GetWithPareceres(Guid id)
        {
            return _context.Afastamentos
                .Include(a => a.Pareceres)
                .FirstOrDefault(a => a.Id == id);
        }

        public Afastamento GetWithDocumentos(Guid id)
        {
            return _context.Afastamentos
                .Include(a => a.Documentos)
                .FirstOrDefault(a => a.Id == id);
        }

        public Afastamento GetWithPareceresAndDocumentos(Guid id)
        {
            return _context.Afastamentos
                .Include(a => a.Pareceres)
                .Include(a => a.Documentos)
                .FirstOrDefault(a => a.Id == id);
        }
    }
}
