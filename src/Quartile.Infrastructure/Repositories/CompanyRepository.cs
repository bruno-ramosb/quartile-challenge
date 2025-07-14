using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;

namespace Quartile.Infrastructure.Repositories
{
    public class CompanyRepository : BaseRepository<Company, Guid>, ICompanyRepository
    {
        public CompanyRepository(QuartileContext context) : base(context)
        {
        }
    }
} 