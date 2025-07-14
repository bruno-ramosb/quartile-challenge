using Microsoft.EntityFrameworkCore;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;

namespace Quartile.Infrastructure.Repositories;

public class StoreRepository : BaseRepository<Store, Guid>, IStoreRepository
{
    private readonly DbSet<Store> _stores;

    public StoreRepository(QuartileContext context) : base(context)
    {
        _stores = context.Stores;
    }

    public async Task<IEnumerable<Store>> GetAllAsync()
    {
        return await _stores
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }
} 