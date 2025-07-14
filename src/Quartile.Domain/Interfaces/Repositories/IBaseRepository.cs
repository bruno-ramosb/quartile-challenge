using Quartile.Domain.Interfaces.Entities;

namespace Quartile.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity, in TKey> : IDisposable
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
    {
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Remove(TEntity entity);
        Task<TEntity> GetByIdAsync(TKey id);
    }
}
