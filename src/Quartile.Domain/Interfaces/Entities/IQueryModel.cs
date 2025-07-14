namespace Quartile.Domain.Interfaces.Entities
{
    public interface IQueryModel;

    public interface IQueryModel<out TKey> : IQueryModel where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}
