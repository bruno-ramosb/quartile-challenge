using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
    {
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);
        Task<TEntity> GetByIdAsync(TKey id);
    }
}
