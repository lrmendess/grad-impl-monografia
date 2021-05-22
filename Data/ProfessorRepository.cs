using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Linq;

namespace SCAP.Data
{
    public class ProfessorRepository : UserRepository<Professor>, IProfessorRepository
    {
        public ProfessorRepository(ApplicationDbContext context)
            : base(context) { }

        public Professor GetChefe()
        {
            var now = DateTime.Now.Date;

            var mandatosVigentes = _context.Mandatos
                .Include(m => m.Professor)
                .Where(m => now >= m.DataInicio.Date && now <= m.DataFim.Date && m.Professor.Ativo && !m.Interrompido)
                .AsNoTracking()
                .ToList();

            var mandatoChefe = mandatosVigentes
                .Find(mv => mv.TipoMandato == TipoMandato.Chefe);

            var mandatoSubchefe = mandatosVigentes
                .Find(mv => mv.TipoMandato == TipoMandato.Subchefe);

            return mandatoChefe?.Professor ?? mandatoSubchefe?.Professor;
        }

        public bool IsChefe(Professor professor)
        {
            if (String.IsNullOrEmpty(professor?.Id))
                return false;

            var now = DateTime.Now.Date;

            var hasMandato = _context.Mandatos
                .Include(m => m.Professor)
                .Where(m => now >= m.DataInicio.Date && now <= m.DataFim.Date && !m.Interrompido)
                .AsNoTracking()
                .Any(m => m.ProfessorId == professor.Id);

            return hasMandato;
        }

        public Professor GetWithParentescos(string id)
        {
            return _context.Professores
                .Include(p => p.Parentescos)
                    .ThenInclude(pp => pp.Parente)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public Professor GetWithAll(string id)
        {
            return _context.Professores
                .Include(p => p.Parentescos)
                .Include(p => p.ParentescoDe)
                .Include(p => p.Afastamentos)
                .Include(p => p.AfastamentosComoRelator)
                .Include(p => p.Mandatos)
                .Include(p => p.Pareceres)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }

        public Professor GetWithAfastamentos(string id)
        {
            return _context.Professores
                .Include(p => p.Afastamentos)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);
        }
    }
}
