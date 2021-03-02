﻿using Microsoft.EntityFrameworkCore;
using SGP.Domain.Entities;
using SGP.Domain.Repositories;
using SGP.Infrastructure.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGP.Infrastructure.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly SGPContext _context;

        public EstadoRepository(SGPContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estado>> GetAllAsync(string siglaPais)
        {
            return await _context.Estados
                .AsNoTracking()
                .Include(e => e.Pais)
                .Where(e => e.Pais.Sigla == siglaPais)
                .OrderBy(e => e.Nome)
                .ThenBy(e => e.Ibge)
                .ToListAsync();
        }
    }
}