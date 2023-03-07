using FluentNHibernate.Mapping;

namespace FluentNhibernateAndMigration.Domain;

public class AccountMap : ClassMap<Account>
{
    public AccountMap()
    {
        Table("accounts");
        Id(x => x.Id);
        Map(x => x.UserName);
        Map(x => x.Email);
    }
}