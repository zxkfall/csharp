using FluentMigrator;

namespace FluentNhibernateAndMigration.migrations;

[Migration(202303072039)]
public class Migration_202303072039 : Migration
{
    public override void Up()
    {
        Create.Table("AccountCopy")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("UserName").AsString(255).NotNullable()
            .WithColumn("Email").AsString(255).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("AccountCopy");
    }
}