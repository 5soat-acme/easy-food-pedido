using EF.Core.Commons.DomainObjects;

namespace EF.Core.Commons.Repository;

public interface IRepository<TEntity> where TEntity : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}