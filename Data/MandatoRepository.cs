using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCAP.Data
{
    public class MandatoRepository : EntityRepository<Mandato>, IMandatoRepository
    {
        public MandatoRepository(ApplicationDbContext context) : base(context) { }

        public IEnumerable<Mandato> GetAllWithProfessor()
        {
            return _context.Mandatos
                .Include(m => m.Professor)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Mandato> GetMandatosVigentes(DateTime date)
        {
            return _context.Mandatos
                .Include(m => m.Professor)
                .Where(m => date.Date >= m.DataInicio.Date && date.Date <= m.DataFim.Date && !m.Interrompido)
                .AsNoTracking()
                .ToList();
        }

        public Mandato GetWithProfessor(Guid id)
        {
            return _context.Mandatos
                .Include(m => m.Professor)
                .AsNoTracking()
                .FirstOrDefault(m => m.Id == id);
        }
    }
}
