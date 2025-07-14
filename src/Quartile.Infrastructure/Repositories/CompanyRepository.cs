using Microsoft.EntityFrameworkCore;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;

namespace Quartile.Infrastructure.Repositories
{
    public class CompanyRepository : BaseRepository<Company, Guid>, ICompanyRepository
    {
        private readonly DbSet<Company> _companies;

        public CompanyRepository(QuartileContext context) : base(context)
        {
            _companies = context.Companies;
        }

        public async Task<Company?> GetByDocumentNumberAsync(string documentNumber)
        {
            return await _companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DocumentNumber == documentNumber);
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _companies
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
} 