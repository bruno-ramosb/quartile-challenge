namespace Quartile.Domain.Interfaces.Entities
{
    public interface IEntity
    {
    }

    public interface IEntity<TId> : IEntity
    {
        public TId Id { get; set; }
    }
}
