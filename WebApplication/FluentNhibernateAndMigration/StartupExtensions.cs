using System.Reflection;
using FluentMigrator.Runner;
using FluentNhibernateAndMigration.Repository;

namespace FluentNhibernateAndMigration;

public static class StartupExtensions
{
    public static void AddFluentMigrator(this IServiceCollection services, string connectionString)
    {
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SqlServer support to FluentMigrator
                .AddSqlServer2016()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }
    
    public static void AddFluentNhibernate(this IServiceCollection services, string connectionString)
    {
        var sessionFactory = MySqlServerSessionFactory.AddDbSessionFactory(connectionString);
        services.AddSingleton(sessionFactory);
        services.AddScoped(factory => sessionFactory.OpenSession());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    }
    
    public static void UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
        runner.ListMigrations();
        runner.MigrateUp();
        // runner.MigrateUp(202303072039);
        // runner.MigrateDown(202303072039);
    }
}