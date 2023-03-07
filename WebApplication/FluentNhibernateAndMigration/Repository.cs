using ISession = NHibernate.ISession;

namespace FluentNhibernateAndMigration;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ISession _session;

    public Repository(ISession session)
    {
        _session = session;
    }

    public async Task Save(T entity)
    {
        using var transaction = _session.BeginTransaction();
        await _session.SaveAsync(entity);
        await transaction.CommitAsync();
    }

    public async Task Delete(T entity)
    {
        using var transaction = _session.BeginTransaction();
        await _session.DeleteAsync(entity);
        await transaction.CommitAsync();
    }

    public async Task Update(T entity)
    {
        using var transaction = _session.BeginTransaction();
        await _session.UpdateAsync(entity);
        await transaction.CommitAsync();
    }

    public async Task SaveOrUpdate(T entity)
    {
        using var transaction = _session.BeginTransaction();
        await _session.SaveOrUpdateAsync(entity);
        await transaction.CommitAsync();
    }


    public IQueryable<T> GetAll()
    {
        return _session.Query<T>();
    }
}