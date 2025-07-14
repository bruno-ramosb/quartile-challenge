namespace Quartile.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        Task<int> CommitAsync(CancellationToken cancellationToken);
        void Rollback();
    }
}
