using Quartile.Domain.Interfaces.Repositories;
using Quartile.Infrastructure.Context;

namespace Quartile.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuartileContext _dbContext;
        private bool disposed;

        public UnitOfWork(QuartileContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public async Task<int> Commit(CancellationToken cancellationToken) => 
            await _dbContext.SaveChangesAsync(cancellationToken);
        

        public void Rollback()
        {
            _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                _dbContext.Dispose();

            disposed = true;
        }


    }
}
