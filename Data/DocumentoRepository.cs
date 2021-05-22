using Microsoft.EntityFrameworkCore;
using SCAP.Data.Interfaces;
using SCAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Data
{
    public class DocumentoRepository :  EntityRepository<Documento>, IDocumentoRepository 
    {
        public DocumentoRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
