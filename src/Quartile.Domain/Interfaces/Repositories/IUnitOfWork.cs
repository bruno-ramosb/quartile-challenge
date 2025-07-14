namespace Quartile.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction();
        Task<int> Commit(CancellationToken cancellationToken);
        void Rollback();
    }
}
