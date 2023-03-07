using FluentNHibernate.Mapping;

namespace FluentNhibernateAndMigration;

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