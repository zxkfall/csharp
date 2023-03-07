using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace FluentNhibernateAndMigration;

public static class MySqlServerSessionFactory
{
    public static ISessionFactory AddDbSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012.ShowSql().ConnectionString(connectionString))
            .Mappings(x => x.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
            .BuildSessionFactory();
    }
}