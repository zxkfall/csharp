namespace FluentNhibernateAndMigration.Repository;

public interface IRepository<T> where T : class
{
    Task Save(T entity);
    Task Delete(T entity);
    Task Update(T entity);
    Task SaveOrUpdate(T entity);
    IQueryable<T> GetAll();
}