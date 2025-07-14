using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Quartile.Infrastructure.Context;

namespace Quartile.Tests.Fixtures
{
    public class EfSqliteFixture : IAsyncLifetime, IDisposable
    {
        private const string ConnectionString = "Data Source=:memory:";
        private readonly SqliteConnection _connection;

        public EfSqliteFixture()
        {
            _connection = new SqliteConnection(ConnectionString);
            _connection.Open();

            var builder = new DbContextOptionsBuilder<QuartileContext>().UseSqlite(_connection);
            Context = new QuartileContext(builder.Options);
        }

        public QuartileContext Context { get; }

        #region IAsyncLifetime

        public async Task InitializeAsync()
        {
            await Context.Database.EnsureDeletedAsync();
            await Context.Database.EnsureCreatedAsync();

            await Seed();
        }

        private async Task Seed()
        {

        }

        public Task DisposeAsync() => Task.CompletedTask;

        #endregion

        #region IDisposable

        private bool _disposed;

        ~EfSqliteFixture() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _connection?.Dispose();
                Context?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}
