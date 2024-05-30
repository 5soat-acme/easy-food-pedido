namespace EF.Core.Commons.Repository;

public interface IUnitOfWork
{
    Task<bool> Commit();
}