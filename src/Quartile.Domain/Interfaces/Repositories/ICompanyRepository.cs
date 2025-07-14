using Quartile.Domain.Entities;
using Quartile.Domain.Enums;

namespace Quartile.Domain.Interfaces.Repositories
{
    public interface ICompanyRepository : IBaseRepository<Company, Guid>
    {
        Task<Company?> GetByDocumentNumberAsync(string documentNumber);
        Task<IEnumerable<Company>> GetAllAsync();
    }
} 