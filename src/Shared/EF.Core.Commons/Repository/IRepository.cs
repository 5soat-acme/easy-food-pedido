using EF.Core.Commons.DomainObjects;

namespace EF.Core.Commons.Repository;

public interface IRepository<TEntity> : IDisposable where TEntity : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}