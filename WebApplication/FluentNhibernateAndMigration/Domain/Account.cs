namespace FluentNhibernateAndMigration.Domain;

public class Account
{
    public virtual int Id { get; set; }
    public virtual string UserName { get; set; }
    public virtual string Email { get; set; }
}