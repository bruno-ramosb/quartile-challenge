using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Domain.Interfaces.Repositories;

public interface IStoreRepository : IBaseRepository<Store, Guid>
{
    Task<IEnumerable<Store>> GetAllAsync();
} 